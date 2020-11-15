using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Ketchup.Core.Runtime.Server;
using Ketchup.Core.Utilities;

namespace Ketchup.Core.Route.Implementation
{
    public class DefaultServiceRouteProvider : IServiceRouteProvider
    {
        private readonly ConcurrentDictionary<string, ServiceRoute> _concurrent = new ConcurrentDictionary<string, ServiceRoute>();

        private readonly ConcurrentDictionary<string, ServiceRoute> _serviceRoute = new ConcurrentDictionary<string, ServiceRoute>();

        private readonly IServiceEntryManager _serviceEntryManager;
        private readonly IServiceRouteManager _serviceRouteManager;

        public DefaultServiceRouteProvider(IServiceEntryManager serviceEntryManager, IServiceRouteManager serviceRouteManager)
        {
            _serviceEntryManager = serviceEntryManager;
            _serviceRouteManager = serviceRouteManager;
        }

        public async Task RegisterRoutes()
        {
            var addess = NetUtils.GetHostAddress();

            var addressDescriptors = _serviceEntryManager.GetEntries().Select(i =>
            {
                return new ServiceRoute
                {
                    Address = new[] { addess },
                    ServiceDescriptor = i.Descriptor
                };
            }).ToList();

            await _serviceRouteManager.SetRoutesAsync(addressDescriptors);
        }
    }
}