using AutoMapper;
using SortThineLetters.Core.DTOs;
using SortThineLetters.Storage.Entities;
using System;

namespace SortThineLetters.Services.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateTwoSidedMap<MailBoxEO, MailBoxDto>();
        }

        private Tuple<
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
