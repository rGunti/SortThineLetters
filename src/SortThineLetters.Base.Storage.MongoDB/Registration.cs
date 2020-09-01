using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;

namespace SortThineLetters.Base.Storage.MongoDB
{
    public static class Registration
    {
        public static IServiceCollection AddMongoClient(
            this IServiceCollection services,
            string connectionString,
            Action<CommandStartedEvent> queryLogger = null)
        {
            ConventionRegistry.Register("EnumStringConversion", new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            }, t => true);
            BsonSerializer.RegisterIdGenerator(
                typeof(string),
                StringObjectIdGenerator.Instance);

            return services
                .AddSingleton<MongoUrl>(s => new MongoUrl(connectionString))
                .AddSingleton<IMongoClient>(s =>
                {
                    var settings = MongoClientSettings.FromUrl(s.GetRequiredService<MongoUrl>());
                    if (queryLogger != null)
                    {
                        settings.ClusterConfigurator = cb =>
                        {
                            cb.Subscribe(queryLogger);
                        };
                    }
                    return new MongoClient(settings);
                })
                .AddSingleton<IMongoDatabase>(s =>
                {
                    return s.GetRequiredService<IMongoClient>()
                        .GetDatabase(s.GetRequiredService<MongoUrl>().DatabaseName);
                });
        }
    }
}
