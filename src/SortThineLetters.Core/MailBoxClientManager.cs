using Microsoft.Extensions.Logging;
using SortThineLetters.Core.DTOs;
using SortThineLetters.Services.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SortThineLetters.Core
{
    public class MailBoxClientManager : LoggingService<MailBoxClientManager>, IDisposable
    {
        private readonly ILogger<MailBoxClient> _clientLogger;
        private readonly IMailBoxService _mailBoxService;

        private readonly Dictionary<string, MailBoxClient> _clients;
        private readonly Dictionary<string, Task> _tasks;

        public MailBoxClientManager(
            ILogger<MailBoxClientManager> logger,
            ILogger<MailBoxClient> clientLogger,
            IMailBoxService mailBoxService)
            : base(logger)
        {
            _clientLogger = clientLogger;
            _mailBoxService = mailBoxService;

            _clients = new Dictionary<string, MailBoxClient>();
            _tasks = new Dictionary<string, Task>();
        }

        public void Initialize()
        {
            foreach (var mailBox in _mailBoxService.GetAll())
            {
                CreateNewClient(mailBox);
            }
        }

        private void AddClient(MailBoxClient client)
        {
            _clients.Add(client.Identifier, client);
            _tasks.Add(client.Identifier, client.Run());
        }

        public void CreateNewClient(MailBoxDto mailBox)
        {
            _logger.LogDebug("Adding client for Mail Box {id} ...", mailBox.Id);
            var client = new MailBoxClient(_clientLogger, mailBox);
            AddClient(client);
        }

        public void Dispose()
        {
            foreach ((var clientId, var task) in _tasks)
            {
                if (!task?.IsCompleted ?? false)
                {
                    _logger.LogInformation("Terminating Client {id} ...", clientId);
                    if (_clients.TryGetValue(clientId, out var client))
                    {
                        client.Terminate();
                    }
                }
            }
            _tasks.Clear();

            foreach ((var clientId, var client) in _clients)
            {
                _logger.LogInformation("Disposing Client {id} ...", clientId);
                client?.Dispose();
            }
            _clients.Clear();
        }
    }
}
