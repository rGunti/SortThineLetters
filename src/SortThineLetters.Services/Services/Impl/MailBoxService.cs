using AutoMapper;
using SortThineLetters.Base.Services;
using SortThineLetters.Core.DTOs;
using SortThineLetters.Storage.Entities;
using SortThineLetters.Storage.Storage;

namespace SortThineLetters.Services.Services.Impl
{
    public class MailBoxService :
        BaseDtoRepositoryService<MailBoxDto, string, MailBoxEO, IMailBoxRepository>,
        IMailBoxService
    {
        public MailBoxService(
            IMapper mapper,
            IMailBoxRepository repository)
            : base(mapper, repository)
        {
        }
    }
}
