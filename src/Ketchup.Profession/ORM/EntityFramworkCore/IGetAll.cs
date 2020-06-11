using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ketchup.Profession.Domain.Implementation;

namespace Ketchup.Profession.ORM.EntityFramworkCore
{
    public interface IGetAll<TEntity, TPrimaryKey> where TEntity : EntityOfTPrimaryKey<TPrimaryKey>
    {
        IQueryable<TEntity> GetAll();
    }
}
