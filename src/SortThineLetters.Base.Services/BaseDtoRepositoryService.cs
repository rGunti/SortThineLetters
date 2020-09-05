using AutoMapper;
using SortThineLetters.Base.DTOs;
using SortThineLetters.Base.Storage.Entities;
using SortThineLetters.Base.Storage.Repository;
using System.Collections.Generic;

namespace SortThineLetters.Base.Services
{
    public abstract class BaseDtoRepositoryService<TDto, TKey, TEntity, TRepository>
        : IDtoService<TDto, TKey>
        where TDto : IDto<TKey>
        where TEntity : IEntityObject<TKey>
        where TRepository : IRepository<TEntity, TKey>
    {
        protected readonly IMapper _mapper;
        protected readonly TRepository _repository;

        protected BaseDtoRepositoryService(
            IMapper mapper,
            TRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        protected virtual IEnumerable<TDto> Map(IEnumerable<TEntity> entities)
        {
            return _mapper.Map<List<TDto>>(entities);
        }
        protected virtual TDto Map(TEntity entity)
        {
            return _mapper.Map<TDto>(entity);
        }
        protected virtual TEntity Map(TDto dto)
        {
            return _mapper.Map<TEntity>(dto);
        }

        public IEnumerable<TDto> GetAll()
        {
            return Map(_repository.FindAll());
        }

        public TDto GetById(TKey key)
        {
            return Map(_repository.Get(key));
        }

        public TDto Create(TDto dto) => Map(_repository.Create(Map(dto)));

        public TDto Update(TDto dto)
        {
            var eo = Map(dto);
            if (_repository.Get(eo.Id) != null)
            {
                return Map(_repository.Update(eo));
            }
            return default;
        }

        public void Delete(TDto dto)
        {
            DeleteById(dto.Id);
        }

        public void DeleteById(TKey key)
        {
            _repository.Delete(key);
        }
    }
}
