using Microsoft.AspNetCore.Mvc;
using SortThineLetters.Core;
using SortThineLetters.Core.DTOs;
using SortThineLetters.Server.Base;
using SortThineLetters.Services.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace SortThineLetters.Server.Controllers.v1
{
    [ApiController]
    [Route("api/v1/mailbox")]
    [SwaggerTag("Mail Box Configuration")]
    public class MailBoxController : DtoServiceController<IMailBoxService, MailBoxDto, string>
    {
        private readonly MailBoxClientManager _clientManager;

        public MailBoxController(
            IMailBoxService service,
            MailBoxClientManager clientManager)
            : base(service)
        {
            _clientManager = clientManager;
        }

        public override List<MailBoxDto> List()
        {
            return base.List();
        }

        public override ActionResult<MailBoxDto> Get([FromRoute] string id)
        {
            return base.Get(id);
        }

        public override ActionResult<MailBoxDto> Post([FromBody] MailBoxDto entity)
        {
            var response = base.Post(entity);
            var newMailBox = (response.Result as OkObjectResult)?.Value as MailBoxDto;
            if (newMailBox != null)
            {
                _clientManager.CreateNewClient(newMailBox);
            }
            return response;
        }

        public override ActionResult<MailBoxDto> Put([FromRoute] string id, [FromBody] MailBoxDto entity)
        {
            var response = base.Put(id, entity);
            var newMailBox = (response.Result as OkObjectResult)?.Value as MailBoxDto;
            if (newMailBox != null)
            {
                _clientManager.DisposeClient(id);
                _clientManager.CreateNewClient(newMailBox);
            }
            return response;
        }

        public override IActionResult Delete([FromRoute] string id)
        {
            var response = base.Delete(id);
            _clientManager.DisposeClient(id);
            return response;
        }
    }
}
