using System;
using System.Collections.Generic;
using System.Text;
using Ketchup.Profession.Application.DTO;
using Ketchup.Profession.Domain;

namespace Ketchup.Profession.Application
{
    public interface ICurdAppServiceOfTPrimaryKey<TEntity, TEntityDto, TSearch> : ICurdAppService<TEntity, int, TEntityDto, TEntityDto, TEntityDto>
        where TEntity : class, IEntity<int>
        where TEntityDto : EntityDto<int>
        where TSearch : PageDto
    {
        List<TEntityDto> PageSearch(TSearch search);

        /// <summary>
        /// 批量提交
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        bool BatchInsert(List<TEntityDto> dtos);

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        bool BatchUpdate(List<TEntityDto> dtos);
    }
}
