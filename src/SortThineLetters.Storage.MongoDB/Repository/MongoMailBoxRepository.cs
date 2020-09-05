using MongoDB.Driver;
using SortThineLetters.Base.Storage.MongoDB.Repository;
using SortThineLetters.Storage.Entities;
using SortThineLetters.Storage.Storage;

namespace SortThineLetters.Storage.MongoDB.Repository
{
    public class MongoMailBoxRepository : MongoRepository<MailBoxEO, string>, IMailBoxRepository
    {
        public MongoMailBoxRepository(IMongoDatabase mongoDatabase)
            : base(mongoDatabase)
        {
        }
    }
}
