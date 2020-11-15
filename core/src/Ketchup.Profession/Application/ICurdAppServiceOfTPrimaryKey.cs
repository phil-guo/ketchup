using System;
using System.Collections.Generic;
using System.Text;
using Ketchup.Profession.Application.DTO;
using Ketchup.Profession.Domain;

namespace Ketchup.Profession.Application
{
    public interface ICurdAppServiceOfTPrimaryKey<TEntity, TEntityDto, in TSearch> : ICurdAppService<TEntity, int, TEntityDto, TEntityDto, TEntityDto>
        where TEntity : class, IEntity<int>
        where TEntityDto : EntityDto<int>
        where TSearch : PageDto
    {
        PageSearchDto<TEntityDto> PageSearch(TSearch search);
    }
}
