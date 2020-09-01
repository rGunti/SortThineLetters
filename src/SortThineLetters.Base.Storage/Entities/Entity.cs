namespace SortThineLetters.Base.Storage.Entities
{
    public class Entity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }
    }

    public class StringKeyEntity : Entity<string> { }
}
