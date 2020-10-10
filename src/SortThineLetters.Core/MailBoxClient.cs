using AutoMapper;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.Extensions.Logging;
using MimeKit;
using SortThineLetters.Core.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SortThineLetters.Core
{
    public class MailBoxClient : LoggingService<MailBoxClient>, IDisposable
    {
        private readonly MailBoxDto _mailBox;
        private readonly ImapClient _client;
        private readonly IMapper _mapper;

        private readonly List<Email> _messages;

        private readonly CancellationTokenSource _cancel;
        private CancellationTokenSource _done;
        private bool _messagesArrived;

        public MailBoxClient(
            ILogger<MailBoxClient> logger,
            MailBoxDto mailBox,
            IMapper mapper)
            : base(logger)
        {
            _messages = new List<Email>();
            _client = new ImapClient();
            _mailBox = mailBox;
            _mapper = mapper;

            _cancel = new CancellationTokenSource();
        }

        public string Identifier => _mailBox?.Id;
        public bool IsDisposed { get; protected set; }

        public async Task Reconnect()
        {
            if (!_client.IsConnected)
            {
                _logger.LogInformation("{id}: Connecting to Mail Box {server}:{port} ...",
                    Identifier, _mailBox.Server, _mailBox.Port);
                await _client.ConnectAsync(_mailBox.Server, _mailBox.Port,
                    MailKit.Security.SecureSocketOptions.Auto,
                    _cancel.Token);
            }

            if (!_client.IsAuthenticated)
            {
                _logger.LogInformation("{id}: Authenticating Mail Box ...", Identifier);
                await _client.AuthenticateAsync(_mailBox.Username, _mailBox.Password, _cancel.Token);

                _logger.LogInformation("{id}: Connecting to Inbox ...", Identifier);
                await _client.Inbox.OpenAsync(FolderAccess.ReadOnly, _cancel.Token);
            }
        }

        public async Task Disconnect()
        {
            _logger.LogInformation("{id}: Disconnecting ...", Identifier);
            await _client.DisconnectAsync(true);
        }

        public async Task Run()
        {
            try
            {
                await Reconnect();
                await FetchMessageSummaries();
            }
            catch (OperationCanceledException)
            {
                await Disconnect();
                return;
            }

            var inbox = _client.Inbox;
            inbox.CountChanged += Inbox_CountChanged;
            inbox.MessageExpunged += Inbox_MessageExpunged;
            inbox.MessageFlagsChanged += Inbox_MessageFlagsChanged;

            await Idle();

            inbox.MessageFlagsChanged -= Inbox_MessageFlagsChanged;
            inbox.MessageExpunged -= Inbox_MessageExpunged;
            inbox.CountChanged -= Inbox_CountChanged;

            await Disconnect();
        }

        public async Task Idle()
        {
            do
            {
                try
                {
                    await WaitForNewMessages();

                    if (_messagesArrived)
                    {
                        await FetchMessageSummaries();
                        _messagesArrived = false;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            } while (!_cancel.IsCancellationRequested);
        }

        private async Task FetchMessageSummaries()
        {
            IList<IMessageSummary> fetched;
            IList<Email> emails;

            do
            {
                try
                {
                    int startIndex = _messages.Count;
                    fetched = _client.Inbox.Fetch(startIndex, -1,
                        MessageSummaryItems.Full | MessageSummaryItems.UniqueId,
                        _cancel.Token);

                    emails = fetched
                        .Select(i =>
                        {
                            var email = _mapper.Map<Email>(i);
                            if (i.BodyParts != null)
                            {
                                _logger.LogDebug("{id}: Downloading message body of #{index} ...", Identifier, i.Index);
                                var parts = new List<EmailBody>();
                                foreach (var bodyPart in i.BodyParts)
                                {
                                    var body = _client.Inbox.GetBodyPart(i.Index, bodyPart, _cancel.Token);
                                    _logger.LogTrace("{id}: Got body #{index},{bodyPart} ...", Identifier, i.Index, body.ContentId);

                                    var emailBody = new EmailBody();

                                    using var memoryStream = new MemoryStream();
                                    if (body is TextPart textPart)
                                    {
                                        textPart.Content.DecodeTo(memoryStream);
                                    }
                                    else
                                    {
                                        body.WriteTo(memoryStream, _cancel.Token);
                                    }
                                    emailBody.Body = memoryStream.ToArray();
                                    parts.Add(emailBody);
                                }

                                email.BodyParts = parts.ToArray();
                            }
                            return email;
                        })
                        .ToList();
                    break;
                }
                catch (ImapProtocolException)
                {
                    await Reconnect();
                }
                catch (IOException)
                {
                    await Reconnect();
                }
            } while (true);

            _logger.LogDebug("{id}: Downloaded {count} messages", Identifier, fetched.Count);
            _messages.AddRange(emails);
        }

        async Task WaitForNewMessages()
        {
            _logger.LogDebug("{id}: Waiting for new messages ...", Identifier);
            do
            {
                try
                {
                    if (_client.Capabilities.HasFlag(ImapCapabilities.Idle))
                    {
                        _done = new CancellationTokenSource(new TimeSpan(0, 9, 0));
                        try
                        {
                            await _client.IdleAsync(_done.Token, _cancel.Token);
                        }
                        finally
                        {
                            _done.Dispose();
                            _done = null;
                        }
                    }
                    else
                    {
                        await Task.Delay(new TimeSpan(0, 1, 0), _cancel.Token);
                        await _client.NoOpAsync(_cancel.Token);
                    }
                    break;
                }
                catch (ImapProtocolException)
                {
                    _logger.LogWarning("{id}: An IMAP Protocol error occurred, reconnecting ...", Identifier);
                    await Reconnect();
                }
                catch (IOException)
                {
                    _logger.LogWarning("{id}: An IO error occurred, reconnecting ...", Identifier);
                    await Reconnect();
                }
            } while (true);
        }

        private void Inbox_MessageFlagsChanged(object sender, MessageFlagsChangedEventArgs e)
        {
            _logger.LogTrace("{id}: Message flags have changed: #{id} -> {flags}",
                Identifier, e.Index, e.Flags);
        }

        private void Inbox_MessageExpunged(object sender, MessageEventArgs e)
        {
            _logger.LogTrace("{id}: Message was expunged: #{uuid}", Identifier, e.Index);
            if (e.Index < _messages.Count)
            {
                _messages.RemoveAt(e.Index);
            }
        }

        private void Inbox_CountChanged(object sender, EventArgs e)
        {
            var folder = (ImapFolder)sender;
            _logger.LogTrace("{id}: Inbox message count has changed: {old} -> {new}",
                Identifier, _messages.Count, folder.Count);
            if (folder.Count > _messages.Count)
            {
                int arrived = folder.Count - _messages.Count;
                if (arrived > 0)
                {
                    _logger.LogDebug("{id}: {count} new messages arrived", Identifier, arrived);
                }

                _messagesArrived = true;
                _done?.Cancel();
            }
        }

        public void Terminate()
        {
            _logger.LogTrace("{id}: Cancelling operations ...", Identifier);
            _cancel.Cancel();
        }

        public void Dispose()
        {
            _logger.LogTrace("{id}: Disposing Client ...", Identifier);
            _client.Dispose();
            _cancel.Dispose();

            IsDisposed = true;
            _logger.LogDebug("{id}: Client disposed", Identifier);
        }
    }
}
