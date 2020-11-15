using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Profession.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int Commit();
    }
}
