using Microsoft.AspNetCore.Mvc;
using SortThineLetters.Core.DTOs;
using SortThineLetters.Server.Base;
using SortThineLetters.Services.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SortThineLetters.Server.Controllers.v1
{
    [ApiController]
    [Route("api/v1/mailbox")]
    [SwaggerTag("Mail Box Configuration")]
    public class MailBoxController : DtoServiceController<IMailBoxService, MailBoxDto, string>
    {
        public MailBoxController(IMailBoxService service)
            : base(service) { }
    }
}
