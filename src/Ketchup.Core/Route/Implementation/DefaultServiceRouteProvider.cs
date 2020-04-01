using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Ketchup.Core.Route.Implementation
{
    public class DefaultServiceRouteProvider : IServiceRouteProvider
    {
        private readonly ConcurrentDictionary<string, ServiceRoute> _concurrent = new ConcurrentDictionary<string, ServiceRoute>();

        private readonly ConcurrentDictionary<string, ServiceRoute> _serviceRoute = new ConcurrentDictionary<string, ServiceRoute>();

        public Task RegisterRoutes()
        {
            throw new NotImplementedException();
        }
    }
}