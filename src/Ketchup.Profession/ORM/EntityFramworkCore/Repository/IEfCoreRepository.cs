using Ketchup.Profession.Domain.Implementation;
using Ketchup.Profession.Repository;

namespace Ketchup.Profession.ORM.EntityFramworkCore.Repository
{
    public interface IEfCoreRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>, IGetAll<TEntity, TPrimaryKey>
        where TEntity : EntityOfTPrimaryKey<TPrimaryKey>
    {
    }
}
