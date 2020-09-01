using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Core.Events;
using SortThineLetters.Base.Storage.MongoDB;
using SortThineLetters.Storage.MongoDB.Repository;
using SortThineLetters.Storage.Storage;
using System;

namespace SortThineLetters.Storage.MongoDB
{
    public static class MongoRegistration
    {
        public static IServiceCollection AddMongoRepositories(
            this IServiceCollection services,
            string connectionString,
            Action<CommandStartedEvent> queryLogger = null)
        {
            return services
                .AddMongoClient(connectionString, queryLogger)
                .AddSingleton<IMailBoxRepository, MongoMailBoxRepository>();
        }
    }
}
