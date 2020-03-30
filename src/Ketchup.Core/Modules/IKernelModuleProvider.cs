using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Modules
{
    public interface IKernelModuleProvider
    {
        List<KernelModule> Modules { get; }
        void Initialize();
    }
}
