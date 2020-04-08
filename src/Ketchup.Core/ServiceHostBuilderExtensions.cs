using Autofac;
using Ketchup.Core.Modules;
using Ketchup.Core.Utilities;
using Microsoft.AspNetCore.Builder;

namespace Ketchup.Core
{
    public static class ServiceHostBuilderExtensions
    {
        public static IApplicationBuilder UseKetchupServer(this IApplicationBuilder app)
        {
            var kernelModule = ServiceLocator.Current.Resolve<IKernelModuleProvider>();

            kernelModule.ApplicationBuilder = app;

            kernelModule.Initialize();

            return app;
        }

        public static IApplicationBuilder UseKetchupClient(this IApplicationBuilder app)
        {
            return app;
        }

        //public static async Task ConfigureRoute(IContainer container)
        //{
        //    var routeProvider = container.Resolve<IServiceRouteProvider>();
        //    await routeProvider.RegisterRoutes();
        //}
    }
}
