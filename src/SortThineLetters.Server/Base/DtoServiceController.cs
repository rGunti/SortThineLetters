using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SortThineLetters.Base.DTOs;
using SortThineLetters.Base.Services;

namespace SortThineLetters.Server.Base
{
    public abstract class DtoServiceController<TService, TDto, TKey> : ControllerBase
        where TService : IDtoService<TDto, TKey>
        where TDto : IDto<TKey>
    {
        protected readonly TService _service;

        protected DtoServiceController(TService service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual List<TDto> List()
        {
            return _service.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public virtual ActionResult<TDto> Get([FromRoute] TKey id)
        {
            var entity = _service.GetById(id);
            if (entity == null)
            {
                return NotFound(id);
            }
            return Ok(entity);
        }

        [HttpPost]
        public virtual ActionResult<TDto> Post([FromBody] TDto entity)
        {
            return Ok(_service.Create(entity));
        }

        [HttpPut("{id}")]
        public virtual ActionResult<TDto> Put(
            [FromRoute] TKey id,
            [FromBody] TDto entity)
        {
            var existingEntity = _service.GetById(id);
            if (existingEntity == null)
            {
                return NotFound(id);
            }
            return Ok(_service.Update(entity));
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(
            [FromRoute] TKey id)
        {
            _service.DeleteById(id);
            return NoContent();
        }
    }
}
