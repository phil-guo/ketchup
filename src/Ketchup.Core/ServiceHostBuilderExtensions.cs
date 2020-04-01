using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Ketchup.Core.Configurations;
using Ketchup.Core.Route;
using Microsoft.Extensions.Hosting;

namespace Ketchup.Core
{
    public static class ServiceHostBuilderExtensions
    {
        public static IContainer UseServer(this IContainer container)
        {
            var ip = AppConfig.ServerOptions.Ip;
            var port = AppConfig.ServerOptions.Port;

            ConfigureRoute(container).GetAwaiter();

            return container;
        }

        public static async Task ConfigureRoute(IContainer container)
        {
            var routeProvider = container.Resolve<IServiceRouteProvider>();
            await routeProvider.RegisterRoutes();
        }
    }
}
