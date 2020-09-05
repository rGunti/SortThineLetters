namespace SortThineLetters.Base.DTOs
{
    public class Dto<TKey> : IDto<TKey>
    {
        public TKey Id { get; set; }
    }
}
