using MailKit;
using MailKit.Net.Imap;
using Microsoft.Extensions.Logging;
using SortThineLetters.Core.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SortThineLetters.Core
{
    public class MailBoxClient : LoggingService<MailBoxClient>, IDisposable
    {
        private readonly MailBoxDto _mailBox;
        private readonly ImapClient _client;

        private readonly List<IMessageSummary> _messages;

        private readonly CancellationTokenSource _cancel;
        private CancellationTokenSource _done;
        private bool _messagesArrived;

        public MailBoxClient(
            ILogger<MailBoxClient> logger,
            MailBoxDto mailBox)
            : base(logger)
        {
            _messages = new List<IMessageSummary>();
            _client = new ImapClient();
            _mailBox = mailBox;

            _cancel = new CancellationTokenSource();
        }

        public string Identifier => _mailBox?.Id;

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

            do
            {
                try
                {
                    int startIndex = _messages.Count;
                    fetched = _client.Inbox.Fetch(startIndex, -1,
                        MessageSummaryItems.Full | MessageSummaryItems.UniqueId,
                        _cancel.Token);
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

            _messages.AddRange(fetched);
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

            _logger.LogDebug("{id}: Client disposed");
        }
    }

    static class MailBoxRegistration
    {
        public static void Connect(
            this MailBoxDto mailBox,
            IImapClient client,
            CancellationTokenSource cancelToken)
        {
            client.Connect(mailBox.Server, mailBox.Port, MailKit.Security.SecureSocketOptions.Auto, cancelToken.Token);
        }
    }
}
