using Ketchup.Profession.Domain;
using Ketchup.Profession.Domain.Implementation;
using Ketchup.Profession.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Ketchup.Profession.ORM.EntityFramworkCore.UntiOfWork
{
    public interface IEfUnitOfWork : IUnitOfWork
    {
        DbSet<TEntity> CreateSet<TEntity, TPrimaryKey>() where TEntity : EntityOfTPrimaryKey<TPrimaryKey>;

        void SetModify<TEntity, TPrimaryKey>(TEntity entity) where TEntity : EntityOfTPrimaryKey<TPrimaryKey>;

        void SetModify<TEntity, TPrimaryKey>(TEntity entity, string[] inCludeColums)
            where TEntity : EntityOfTPrimaryKey<TPrimaryKey>;
    }
}