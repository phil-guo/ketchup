using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace Ketchup.Core.Modules
{
    public interface IKernelModuleProvider
    {
        List<KernelModule> Modules { get; }

        void Initialize();

        public IApplicationBuilder ApplicationBuilder { get; set; }
    }
}
