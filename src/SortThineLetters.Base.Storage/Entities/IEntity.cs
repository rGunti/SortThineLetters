namespace SortThineLetters.Base.Storage.Entities
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
