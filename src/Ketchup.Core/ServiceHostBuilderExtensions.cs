using Autofac;
using Ketchup.Core.Modules;
using Ketchup.Core.Utilities;
using Microsoft.AspNetCore.Builder;

namespace Ketchup.Core
{
    public static class ServiceHostBuilderExtensions
    {
        public static IApplicationBuilder UseKetchup(this IApplicationBuilder app)
        {
            var kernelModule = ServiceLocator.Current.Resolve<IKernelModuleProvider>();

            kernelModule.ApplicationBuilder = app;

            kernelModule.Initialize();

            return app;
        }
    }
}
