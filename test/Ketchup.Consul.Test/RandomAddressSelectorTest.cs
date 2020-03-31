using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Consul.Selector.Implementation;
using Ketchup.Core;
using Ketchup.Core.Address;
using Ketchup.Core.Address.Selectors.Implementation;
using Xunit;

namespace Ketchup.Consul.Test
{
    public class RandomAddressSelectorTest
    {
        [Fact]
        public async Task RandomSelector_Test()
        {
            var moq = new ConsulRandomAddressSelector();

            var result = await moq.SelectAsync(new AddressSelectContext()
            {
                Address = new List<AddressModel>()
                {
                    new IpAddressModel()
                    {
                        Ip = "127.0.0.1",
                        Port = 8500
                    },
                    new IpAddressModel()
                    {
                        Ip = "192.168.3.11",
                        Port = 8500
                    }
                },
                Descriptor = new ServiceDescriptor()
                {
                    Id = Guid.NewGuid().ToString("N"),
                }
            });

            Assert.NotNull(result);
        }
    }
}
