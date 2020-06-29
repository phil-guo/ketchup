using Ketchup.Profession.Domain.Implementation;
using Ketchup.Profession.ORM.EntityFramworkCore;
using Ketchup.Profession.Repository;

namespace Ketchup.Profession.ORM.FreeSql.Repository
{
    public interface IFreeSqlRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>, IFreeSqlGetAll
        where TEntity : EntityOfTPrimaryKey<TPrimaryKey>
    {
    }
}
