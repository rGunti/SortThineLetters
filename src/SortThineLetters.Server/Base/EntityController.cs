using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SortThineLetters.Base.Storage.Entities;
using SortThineLetters.Base.Storage.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SortThineLetters.Server.Base
{
    public abstract class EntityController<TRepository, TEntity, TKey> : ControllerBase
        where TRepository : IRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
    {
        protected readonly TRepository _repository;

        protected EntityController(TRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public virtual List<TEntity> List()
        {
            return _repository.FindAll().ToList();
        }

        [HttpGet("{id}")]
        public virtual ActionResult<TEntity> Get([FromRoute] TKey id)
        {
            var entity = _repository.Get(id);
            if (entity == null)
            {
                return NotFound(id);
            }
            return Ok(entity);
        }

        [HttpPost]
        public virtual ActionResult<TEntity> Post([FromBody] TEntity entity)
        {
            return Ok(_repository.Create(entity));
        }

        [HttpPut("{id}")]
        public virtual ActionResult<TEntity> Put(
            [FromRoute] TKey id,
            [FromBody] TEntity entity)
        {
            var existingEntity = _repository.Get(id);
            if (existingEntity == null)
            {
                return NotFound(id);
            }
            return Ok(_repository.Update(entity));
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(
            [FromRoute] TKey id)
        {
            _repository.Delete(id);
            return NoContent();
        }
    }
}
