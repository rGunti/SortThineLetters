using Microsoft.AspNetCore.Mvc;
using SortThineLetters.Server.Base;
using SortThineLetters.Storage.Entities;
using SortThineLetters.Storage.Storage;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SortThineLetters.Server.Controllers
{
    [ApiController]
    [Route("api/mailbox")]
    [SwaggerTag("Mail Box Configuration")]
    public class MailBoxController : EntityController<IMailBoxRepository, MailBox, string>
    {
        public MailBoxController(IMailBoxRepository repository)
            : base(repository) { }
    }
}
