namespace SortThineLetters.Base.DTOs
{
    public interface IDto<TKey>
    {
        TKey Id { get; set; }
    }
}
