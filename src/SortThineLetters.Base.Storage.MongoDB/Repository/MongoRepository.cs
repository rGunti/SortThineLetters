using MongoDB.Driver;
using SortThineLetters.Base.Storage.Entities;
using SortThineLetters.Base.Storage.Repository;
using System.Linq;

namespace SortThineLetters.Base.Storage.MongoDB.Repository
{
    public abstract class MongoRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : IEntityObject<TKey>
    {
        protected readonly IMongoDatabase _mongoDatabase;

        protected MongoRepository(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;

            var entityName = typeof(TEntity).Name;
            if (entityName.EndsWith("EO"))
            {
                entityName = entityName[0..^2];
            }
            CollectionName = entityName;
        }

        protected string CollectionName { get; }

        protected IMongoCollection<TEntity> Collection => _mongoDatabase.GetCollection<TEntity>(CollectionName);

        protected FilterDefinitionBuilder<TEntity> Filter => Builders<TEntity>.Filter;

        protected virtual FilterDefinition<TEntity> GetIdFilter(TKey key)
        {
            return Filter.Eq(e => e.Id, key);
        }

        public virtual IQueryable<TEntity> FindAll()
        {
            return Collection.AsQueryable();
        }

        public virtual TEntity Get(TKey key)
        {
            return Collection.Find(GetIdFilter(key))
                .SingleOrDefault();
        }

        public virtual TEntity Create(TEntity entity)
        {
            Collection.InsertOne(entity);
            return Get(entity.Id);
        }

        public virtual TEntity Update(TEntity entity)
        {
            Collection.ReplaceOne(GetIdFilter(entity.Id), entity);
            return Get(entity.Id);
        }

        public virtual void Delete(TKey key)
        {
            Collection.DeleteOne(GetIdFilter(key));
        }
    }
}
