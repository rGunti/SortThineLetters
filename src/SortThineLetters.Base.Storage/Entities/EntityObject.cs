namespace SortThineLetters.Base.Storage.Entities
{
    public class EntityObject<TKey> : IEntityObject<TKey>
    {
        public TKey Id { get; set; }
    }
}
