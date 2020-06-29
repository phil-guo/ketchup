using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Profession.ORM.FreeSql.UnitOfWork
{
    public interface IFreeSqlUnitOfWork
    {
        IFreeSql GetSet();
    }
}
