using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Ketchup.Profession.Application.DTO;
using Ketchup.Profession.AutoMapper;
using Ketchup.Profession.Domain;
using Ketchup.Profession.ORM.EntityFramworkCore;
using Ketchup.Profession.Repository;
using Microsoft.EntityFrameworkCore;
using IObjectMapper = Ketchup.Profession.AutoMapper.ObjectMapper.IObjectMapper;

namespace Ketchup.Profession.Application.Implementation
{
    public abstract class CurdAppServiceOfTPrimaryKey<TEntity, TEntityDto, TSearch> :
        CurdAppService<TEntity, int, TEntityDto, TEntityDto, TEntityDto>,
        ICurdAppServiceOfTPrimaryKey<TEntity, TEntityDto, TSearch>
        where TEntity : class, IEntity<int>
        where TEntityDto : EntityDto<int>
        where TSearch : PageDto
    {
        private readonly IGetAll<TEntity, int> _getAll;
        private readonly IMapper _mapper;

        protected CurdAppServiceOfTPrimaryKey(IRepository<TEntity, int> repository,
            IObjectMapper objectMapper,
            IGetAll<TEntity, int> getAll, IMapper mapper) : base(
            repository, objectMapper)
        {
            _getAll = getAll;
            _mapper = mapper;
        }

        public virtual PageSearchDto<TEntityDto> PageSearch(TSearch search)
        {
            var query = _getAll.GetAll().AsNoTracking();

            if (SearchFilter(search) != null)
                query = query.Where(SearchFilter(search));

            query = OrderFilter() != null
                ? query.OrderByDescending(OrderFilter())
                : query.OrderByDescending(item => item.Id);

            var total = query.Count();

            var result = query.Skip(search.PageSize * (search.PageIndex - 1))
            .Take(search.PageSize)
            .ToList();

            return new PageSearchDto<TEntityDto>()
            {
                EntityDtos = ConvertToEntities(result),
                Total = total
            };
        }

        protected virtual Expression<Func<TEntity, bool>> SearchFilter(TSearch search)
        {
            return null;
        }

        protected virtual Expression<Func<TEntity, int>> OrderFilter()
        {
            return null;
        }

        protected virtual List<TEntityDto> ConvertToEntities(List<TEntity> entities)
        {
            return _mapper.MapTo<List<TEntityDto>>(entities);
        }
    }
}