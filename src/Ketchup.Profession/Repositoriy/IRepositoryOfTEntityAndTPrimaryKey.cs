using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Ketchup.Profession.Domain;

namespace Ketchup.Profession.Repositoriy
{
    public interface IRepositoryOfTEntityAndTPrimaryKey<TEntity, in TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        TEntity Insert(TEntity entity);

        TEntity Update(TEntity entity);

        List<TEntity> GetAllList();

        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefault(TPrimaryKey id);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Delete(TPrimaryKey id);

        void Delete(Expression<Func<TEntity, bool>> predicate);

        int Count();

        int Count(Expression<Func<TEntity, bool>> predicate);
    }
}
