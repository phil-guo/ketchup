using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ketchup.Profession.Application.DTO;
using Ketchup.Profession.AutoMapper.ObjectMapper;
using Ketchup.Profession.Domain;
using Ketchup.Profession.Repository;

namespace Ketchup.Profession.Application.Implementation
{
    /// <summary>
    ///     增删改查基础实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <typeparam name="TCreateEntityDto"></typeparam>
    /// <typeparam name="TUpdateEntityDto"></typeparam>
    /// <typeparam name="TEntityDto"></typeparam>
    public abstract class CurdAppService<TEntity, TPrimaryKey, TEntityDto, TCreateEntityDto, TUpdateEntityDto> :
        ICurdAppService<TEntity, TPrimaryKey, TEntityDto, TCreateEntityDto, TUpdateEntityDto>
        where TEntity : class, IEntity<TPrimaryKey>
        where TUpdateEntityDto : EntityDto<TPrimaryKey>
        where TEntityDto : EntityDto<TPrimaryKey>
    {
        public readonly IRepository<TEntity, TPrimaryKey> Repository;
        public readonly IObjectMapper ObjectMapper;

        protected CurdAppService(IRepository<TEntity, TPrimaryKey> repository, IObjectMapper objectMapper)
        {
            Repository = repository;
            ObjectMapper = objectMapper;
        }

        /// <summary>
        ///     添加
        /// </summary>
        /// <param name="dto">数据传输对象</param>
        /// <returns>实体</returns>
        public virtual TEntityDto Insert(TCreateEntityDto dto)
        {
            var entity = MapCreateToEntity(dto);
            entity = Repository.Insert(entity);
            return entity == null ? null : MapToEntityDto(entity);
        }

        /// <summary>
        ///     更新
        /// </summary>
        /// <param name="dto">数据传输对象</param>
        /// <returns>实体</returns>
        public virtual TEntityDto Update(TUpdateEntityDto dto)
        {
            var entity = FirstOrDefault(dto.Id);
            MapToEntity(dto, entity);
            entity = Repository.Update(entity);
            return entity == null ? null : MapToEntityDto(entity);
        }

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="id">主键</param>
        public virtual void Delete(TPrimaryKey id)
        {
            Repository.Delete(id);
        }

        /// <summary>
        ///     查询一个实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public virtual TEntity FirstOrDefault(TPrimaryKey id)
        {
            return Repository.FirstOrDefault(id);
        }

        /// <summary>
        ///     查询一个实体
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns>实体</returns>
        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Repository.FirstOrDefault(predicate);
        }

        protected virtual TEntity MapCreateToEntity(TCreateEntityDto createInput)
        {
            return ObjectMapper.Map<TEntity>(createInput);
        }
        protected virtual void MapToEntity(TUpdateEntityDto updateInput, TEntity entity)
        {
            ObjectMapper.Map(updateInput, entity);
        }
        protected virtual TEntityDto MapToEntityDto(TEntity entity)
        {
            return ObjectMapper.Map<TEntityDto>(entity);
        }

        protected virtual TEntity MapToEntity(TEntityDto entityDto)
        {
            return ObjectMapper.Map<TEntity>(entityDto);
        }
    }

    //public abstract class CurdAppService<TEntity, TEntityDto, TCreateEntityDto, TUpdateEntityDto> :
    //    CurdAppService<TEntity, int, TEntityDto, TCreateEntityDto, TUpdateEntityDto>,
    //    ICurdAppService<TEntity, TEntityDto, TCreateEntityDto, TUpdateEntityDto>
    //    where TEntity : class, IEntity<int>
    //    where TUpdateEntityDto : EntityDto<int>
    //    where TEntityDto : EntityDto<int>
    //{
    //    public readonly IRepository<TEntity, int> Repository;

    //    protected CurdAppService(IObjectMapper objectMapper, IRepository<TEntity, int> repository) : base(objectMapper)
    //    {
    //        Repository = repository;
    //    }
    //    public bool BatchInsert(List<TCreateEntityDto> dtos)
    //    {
    //        var entities = new List<TEntity>();
    //        dtos.ForEach(dto =>
    //        {
    //            entities.Add(MapCreateToEntity(dto));
    //        });
    //        return Repository.BatchInsert(entities);
    //    }

    //    public bool BatchUpdate(List<TEntityDto> dtos)
    //    {
    //        var entities = new List<TEntity>();
    //        dtos.ForEach(dto =>
    //        {
    //            entities.Add(MapToEntity(dto));
    //        });
    //        return Repository.BatchUpdate(entities);
    //    }
    //}
}