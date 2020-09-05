using SortThineLetters.Base.DTOs;
using System.Collections.Generic;

namespace SortThineLetters.Base.Services
{
    public interface IDtoService<TDto, TKey>
        where TDto : IDto<TKey>
    {
        IEnumerable<TDto> GetAll();
        TDto GetById(TKey key);

        TDto Create(TDto dto);
        TDto Update(TDto dto);

        void Delete(TDto dto);
        void DeleteById(TKey key);
    }
}
