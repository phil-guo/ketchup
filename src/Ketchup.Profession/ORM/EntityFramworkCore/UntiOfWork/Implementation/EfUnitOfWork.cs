using System;
using System.Linq;
using Ketchup.Profession.Domain.Implementation;
using Ketchup.Profession.ORM.EntityFramworkCore.Context;
using Microsoft.EntityFrameworkCore;

namespace Ketchup.Profession.ORM.EntityFramworkCore.UntiOfWork.Implementation
{
    public class EfUnitOfWork<TContext> : IEfUnitOfWork
        where TContext : DbContext, IEfCoreContext
    {

        public readonly TContext _defaultDbContext;

        public EfUnitOfWork(TContext defaultDbContext)
        {
            _defaultDbContext = defaultDbContext;
        }

        public void Dispose()
        {
            _defaultDbContext?.Dispose();
        }

        public int Commit()
        {
            try
            {
                if (_defaultDbContext == null)
                    return 0;
                return _defaultDbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DbSet<TEntity> CreateSet<TEntity, TPrimaryKey>() where TEntity : EntityOfTPrimaryKey<TPrimaryKey>
        {
            return _defaultDbContext?.CreateSet<TEntity, TPrimaryKey>();
        }

        public void SetModify<TEntity, TPrimaryKey>(TEntity entity) where TEntity : EntityOfTPrimaryKey<TPrimaryKey>
        {
            if (_defaultDbContext == null)
                return;
            _defaultDbContext.Entry(entity).State = EntityState.Modified;
        }

        public void SetModify<TEntity, TPrimaryKey>(TEntity entity, string[] inCludeColums) where TEntity : EntityOfTPrimaryKey<TPrimaryKey>
        {
            var dbEntityEntry = _defaultDbContext.Entry(entity);
            if (!inCludeColums.Any())
                return;

            _defaultDbContext.Entry(entity).State = EntityState.Modified;
            inCludeColums?.ToList().ForEach(colums =>
            {
                dbEntityEntry.OriginalValues.Properties.ToList().ForEach(property =>
                {
                    if (property.Name == colums)
                        _defaultDbContext.Entry(entity).Property(property.Name).IsModified = true;
                });
            });
        }
    }
}
