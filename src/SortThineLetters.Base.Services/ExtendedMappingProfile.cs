using AutoMapper;
using System;

namespace SortThineLetters.Base.Services
{
    public abstract class ExtendedMappingProfile : Profile
    {
        protected Tuple<
            IMappingExpression<TEntity, TDto>,
            IMappingExpression<TDto, TEntity>
            > CreateTwoSidedMap<TEntity, TDto>()
        {
            return new Tuple<IMappingExpression<TEntity, TDto>, IMappingExpression<TDto, TEntity>>(
                CreateMap<TEntity, TDto>(),
                CreateMap<TDto, TEntity>());
        }
    }
}
