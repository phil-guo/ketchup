using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ketchup.Profession.Application.DTO;
using Ketchup.Profession.Domain;

namespace Ketchup.Profession.Application
{
    public interface ICurdAppService<TEntity, in TPrimaryKey, out TEntityDto, in TCreateEntityDto, in TUpdateEntityDto>
        where TEntity : class, IEntity<TPrimaryKey>
        where TUpdateEntityDto : EntityDto<TPrimaryKey>
        where TEntityDto : EntityDto<TPrimaryKey>
    {
        /// <summary>
        ///     添加
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TEntityDto Insert(TCreateEntityDto dto);

        /// <summary>
        ///     修改
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TEntityDto Update(TUpdateEntityDto dto);

        /// <summary>
        ///     物理删除
        /// </summary>
        /// <param name="id"></param>
        void Delete(TPrimaryKey id);

        /// <summary>
        ///     根据id 获取一个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity FirstOrDefault(TPrimaryKey id);

        /// <summary>
        ///     根据条件获取一个实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    }

    //public interface ICurdAppService<TEntity, TEntityDto, TCreateEntityDto, in TUpdateEntityDto>
    //    : ICurdAppService<TEntity, int, TEntityDto, TCreateEntityDto, TUpdateEntityDto>
    //    where TEntity : class, IEntity<int>
    //    where TUpdateEntityDto : EntityDto<int>
    //    where TEntityDto : EntityDto<int>
    //{
    //    /// <summary>
    //    /// 批量提交
    //    /// </summary>
    //    /// <param name="dtos"></param>
    //    /// <returns></returns>
    //    bool BatchInsert(List<TCreateEntityDto> dtos);

    //    /// <summary>
    //    /// 批量修改
    //    /// </summary>
    //    /// <param name="dtos"></param>
    //    /// <returns></returns>
    //    bool BatchUpdate(List<TEntityDto> dtos);
    //}
}