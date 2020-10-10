using SortThineLetters.Base.Services;
using SortThineLetters.Core.DTOs;
using SortThineLetters.Storage.Entities;

namespace SortThineLetters.Services.Mapper
{
    public class MappingProfile : ExtendedMappingProfile
    {
        public MappingProfile()
        {
            CreateTwoSidedMap<MailBoxEO, MailBoxDto>();
        }
    }
}
