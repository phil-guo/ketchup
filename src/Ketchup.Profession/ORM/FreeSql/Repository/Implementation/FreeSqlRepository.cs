using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ketchup.Profession.Domain.Implementation;
using Ketchup.Profession.ORM.FreeSql.UnitOfWork;

namespace Ketchup.Profession.ORM.FreeSql.Repository.Implementation
{
    public class FreeSqlRepository<TEntity, TPrimaryKey> : IFreeSqlRepository<TEntity, TPrimaryKey>, IFreeSqlGetAll
        where TEntity : EntityOfTPrimaryKey<TPrimaryKey>
    {
        private readonly IFreeSqlUnitOfWork _freeSql;

        public FreeSqlRepository(IFreeSqlUnitOfWork freeSql)
        {
            _freeSql = freeSql;
        }

        public TEntity Insert(TEntity entity)
        {
            return GetAll().GetRepository<TEntity>().Insert(entity);
        }

        public TEntity Update(TEntity entity)
        {
            try
            {
                var repos = GetAll().Update<TEntity>().SetSource(entity).ExecuteAffrows();
                return repos > 0 ? entity : null;
            }
            catch (Exception e)
            {
                throw new Exception("ef core add error:" + e.Message);
            }
        }

        public List<TEntity> GetAllList()
        {
            return GetAll().Select<TEntity>().ToList();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Select<TEntity>().Where(predicate).ToList();
        }

        public TEntity FirstOrDefault(TPrimaryKey id)
        {
            return GetAll().Select<TEntity>().Where(CreateEqualityExpressionForId(id)).ToOne();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Select<TEntity>().Where(predicate).ToOne();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Select<TEntity>().Where(predicate).ToOne();
        }

        public void Delete(TPrimaryKey id)
        {
            GetAll().Delete<TEntity>().Where(CreateEqualityExpressionForId(id)).ExecuteAffrows();
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            GetAll().Delete<TEntity>().Where(predicate).ExecuteAffrows();
        }

        public int Count()
        {
            return Convert.ToInt32(GetAll().Select<TEntity>().Count());
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Convert.ToInt32(GetAll().Select<TEntity>().Where(predicate).Count());
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

        public IFreeSql GetAll()
        {
            return _freeSql.GetSet();
        }
    }
}
