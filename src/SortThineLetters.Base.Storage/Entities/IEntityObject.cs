namespace SortThineLetters.Base.Storage.Entities
{
    public interface IEntityObject<TKey>
    {
        TKey Id { get; set; }
    }
}
