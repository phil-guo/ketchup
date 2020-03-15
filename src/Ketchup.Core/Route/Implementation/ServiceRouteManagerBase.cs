using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Core.Address;
using Ketchup.Core.Route.Implementation.Args;
using Ketchup.Core.Serialization;

namespace Ketchup.Core.Route.Implementation
{
    public abstract class ServiceRouteManagerBase : IServiceRouteManager
    {
        private EventHandler<ServiceRouteEventArgs> _created;
        private EventHandler<ServiceRouteEventArgs> _removed;
        private EventHandler<ServiceRouteChangedEventArgs> _changed;
        private readonly ISerializer<string> _serializer;

        public ServiceRouteManagerBase(ISerializer<string> serializer)
        {
            _serializer = serializer;
        }


        public event EventHandler<ServiceRouteEventArgs> Created
        {
            add => _created += value;
            remove => _created -= value;
        }
        public event EventHandler<ServiceRouteEventArgs> Removed
        {
            add => _removed += value;
            remove => _removed -= value;
        }
        public event EventHandler<ServiceRouteChangedEventArgs> Changed
        {
            add => _changed += value;
            remove => _changed -= value;
        }

        public abstract Task<IEnumerable<ServiceRoute>> GetRoutesAsync();


        public virtual Task SetRoutesAsync(IEnumerable<ServiceRoute> routes)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));

            var descriptors = routes.Where(route => route != null).Select(route => new ServiceRouteDescriptor
            {
                AddressDescriptors = route.Address?.Select(address => new ServiceAddressDescriptor
                {
                    Value = _serializer.Serialize(address)
                }) ?? Enumerable.Empty<ServiceAddressDescriptor>(),
                ServiceDescriptor = route.ServiceDescriptor
            });

            return SetRoutesAsync(descriptors);
        }

        public abstract Task RemveAddressAsync(IEnumerable<AddressModel> Address);
        public abstract Task ClearAsync();

        protected abstract Task SetRoutesAsync(IEnumerable<ServiceRouteDescriptor> routes);
    }
}
