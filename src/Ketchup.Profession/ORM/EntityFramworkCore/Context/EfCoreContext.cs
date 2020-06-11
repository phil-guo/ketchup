using System.Collections.Concurrent;
using Ketchup.Profession.Domain;
using Ketchup.Profession.Domain.Implementation;
using Microsoft.EntityFrameworkCore;

namespace Ketchup.Profession.ORM.EntityFramworkCore.Context
{
    public abstract class EfCoreContext : DbContext
    {
        private readonly ConcurrentDictionary<string, object> _allSet = new ConcurrentDictionary<string, object>();

        public virtual DbSet<TEntity> CreateSet<TEntity, TPrimaryKey>()
            where TEntity : EntityOfTPrimaryKey<TPrimaryKey>
        {
            var key = typeof(TEntity).FullName;
            object result;

            if (!_allSet.TryGetValue(key, out result))
            {
                result = Set<TEntity>();
                _allSet.TryAdd(key, result);
            }
            return Set<TEntity>();
        }
    }
}
