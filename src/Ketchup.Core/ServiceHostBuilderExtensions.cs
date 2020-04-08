using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ketchup.Core.Configurations;
using Ketchup.Core.Modules;
using Ketchup.Core.Route;
using Ketchup.Core.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

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

        //public static async Task ConfigureRoute(IContainer container)
        //{
        //    var routeProvider = container.Resolve<IServiceRouteProvider>();
        //    await routeProvider.RegisterRoutes();
        //}
    }
}
