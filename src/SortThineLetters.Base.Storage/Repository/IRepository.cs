using SortThineLetters.Base.Storage.Entities;
using System.Linq;

namespace SortThineLetters.Base.Storage.Repository
{
    public interface IRepository<TEntity, TKey>
        where TEntity : IEntityObject<TKey>
    {
        IQueryable<TEntity> FindAll();
        TEntity Get(TKey key);

        TEntity Create(TEntity entity);
        TEntity Update(TEntity entity);
        void Delete(TKey key);
    }
}
