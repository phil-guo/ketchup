using System;
using System.Collections.Generic;
using Ketchup.Profession.Application.DTO;
using Ketchup.Profession.AutoMapper.ObjectMapper;
using Ketchup.Profession.Domain;
using Ketchup.Profession.ORM.EntityFramworkCore;
using Ketchup.Profession.Repository;

namespace Ketchup.Profession.Application.Implementation
{
    public class CurdAppServiceOfTPrimaryKey<TEntity, TEntityDto, TSearch> :
        CurdAppService<TEntity, int, TEntityDto, TEntityDto, TEntityDto>,
        ICurdAppServiceOfTPrimaryKey<TEntity, TEntityDto, TSearch>
        where TEntity : class, IEntity<int>
        where TEntityDto : EntityDto<int>
        where TSearch : PageDto
    {
        private readonly IGetAll<TEntity, int> _getAll;

        public CurdAppServiceOfTPrimaryKey(IRepository<TEntity, int> repository, IObjectMapper objectMapper) : base(
            repository, objectMapper)
        {
        }

        public List<TEntityDto> PageSearch(TSearch search)
        {
            throw new NotImplementedException();
        }

        public bool BatchInsert(List<TEntityDto> dtos)
        {
            throw new NotImplementedException();
        }

        public bool BatchUpdate(List<TEntityDto> dtos)
        {
            throw new NotImplementedException();
        }
    }
}