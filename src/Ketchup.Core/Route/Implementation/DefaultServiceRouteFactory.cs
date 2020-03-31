using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Core.Address;
using Ketchup.Core.Serialization;

namespace Ketchup.Core.Route.Implementation
{
    /// <summary>
    /// 默认的服务路由工厂实现
    /// </summary>
    public class DefaultServiceRouteFactory : IServiceRouteFactory
    {
        private readonly ConcurrentDictionary<string, AddressModel> _addressModel = new ConcurrentDictionary<string, AddressModel>();
        private readonly ISerializer<string> _serializer;

        public DefaultServiceRouteFactory(ISerializer<string> serializer)
        {
            _serializer = serializer;
        }

        public Task<IEnumerable<ServiceRoute>> CreateServiceRoutesAsync(IEnumerable<ServiceRouteDescriptor> descriptors)
        {
            if (descriptors == null)
                throw new ArgumentNullException(nameof(descriptors));

            descriptors = descriptors.ToArray();
            var routes = new List<ServiceRoute>(descriptors.Count());

            routes.AddRange(descriptors.Select(descriptor => new ServiceRoute
            {

                Address = CreateAddress(descriptor.AddressDescriptors),
                ServiceDescriptor = descriptor.ServiceDescriptor
            }));

            return Task.FromResult(routes.AsEnumerable());
        }

        private IEnumerable<AddressModel> CreateAddress(IEnumerable<ServiceAddressDescriptor> descriptors)
        {
            if (descriptors == null)
                yield break;

            foreach (var descriptor in descriptors)
            {
                _addressModel.TryGetValue(descriptor.Value, out AddressModel address);
                if (address == null)
                {
                    address = (AddressModel)_serializer.Deserialize(descriptor.Value, typeof(IpAddressModel));
                    _addressModel.TryAdd(descriptor.Value, address);
                }
                yield return address;
            }
        }
    }
}
