using System;
using System.Collections.Generic;
using System.Text;
using Ketchup.Profession.Domain.Implementation;
using Microsoft.EntityFrameworkCore;

namespace Ketchup.Profession.ORM.EntityFramworkCore.Context
{
    public interface IEfCoreContext
    {
        DbSet<TEntity> CreateSet<TEntity, TPrimaryKey>() where TEntity : EntityOfTPrimaryKey<TPrimaryKey>;
    }
}
