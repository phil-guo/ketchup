using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ketchup.Profession.Domain.Implementation;
using Ketchup.Profession.ORM.EntityFramworkCore.UntiOfWork;
using Microsoft.EntityFrameworkCore;

namespace Ketchup.Profession.ORM.EntityFramworkCore.Repository.Implementation
{
    public class EfCoreRepository<TEntity, TPrimaryKey> : IEfCoreRepository<TEntity, TPrimaryKey>
        where TEntity : EntityOfTPrimaryKey<TPrimaryKey>
    {
        private readonly IEfUnitOfWork _unitOfWork;

        public EfCoreRepository(IEfUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public TEntity Insert(TEntity entity)
        {
            try
            {
                GetSet().Add(entity);
                return _unitOfWork.Commit() <= 0 ? null : entity;
            }
            catch (Exception e)
            {
                throw new Exception("ef core add error:" + e.Message);
            }
        }

        public TEntity Update(TEntity entity)
        {
            try
            {
                _unitOfWork.SetModify<TEntity, TPrimaryKey>(entity);
                return _unitOfWork.Commit() <= 0 ? null : entity;
            }
            catch (Exception e)
            {
                throw new Exception("ef core modify error:" + e.Message);
            }
        }

        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public TEntity FirstOrDefault(TPrimaryKey id)
        {
            return GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().SingleOrDefault(predicate);
        }

        public void Delete(TPrimaryKey id)
        {
            var entity = SingleOrDefault(CreateEqualityExpressionForId(id));

            if (entity == null)
                return;
            Delete(entity);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var source in GetAll().Where(predicate).ToList())
            {
                Delete(source);
            }
        }

        public void Delete(TEntity entity)
        {
            try
            {
                GetSet().Remove(entity);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                throw new Exception("ef core delete error:" + e.Message);
            }
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public IQueryable<TEntity> GetAll()
        {
            return GetSet();
        }

        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
            );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        private DbSet<TEntity> GetSet()
        {
            return _unitOfWork.CreateSet<TEntity, TPrimaryKey>();
        }
    }
}
