using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Ketchup.Consul.Internal.Selector.Implementation;
using Ketchup.Core;
using Ketchup.Core.Address;
using Ketchup.Core.Address.Selectors.Implementation;
using Xunit;

namespace Ketchup.Consul.Test
{
    public class AddressSelectorTest
    {
        [Fact(DisplayName = "随机选择测试")]
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
            });

            Assert.NotNull(result);
        }

        [Fact(DisplayName = "轮询地址选择算法")]
        public async Task PollingSelector_Test()
        {
            var moq = AutoMock.GetLoose().Create<PollingAddressSelector>();

            for (int i = 0; i < 3; i++)
            {
                var result = await moq.SelectAsync(new AddressSelectContext()
                {
                    Address = new List<AddressModel>() { new IpAddressModel()
                        {
                            Ip = "127.0.0.1",
                            Port = 8000
                        },
                        new IpAddressModel()
                        {
                            Ip = "192.168.3.11",
                            Port = 8500
                        }},
                    Name = "user"
                });

                var one = result as IpAddressModel;

                switch (i)
                {
                    case 0:
                        Assert.Equal("127.0.0.1:8000", $"{one.Ip}:{one.Port}");
                        break;
                    case 1:
                        Assert.Equal("192.168.3.11:8500", $"{one.Ip}:{one.Port}");
                        break;
                    case 2:
                        Assert.Equal("127.0.0.1:8000", $"{one.Ip}:{one.Port}");
                        break;
                }
            }
        }
    }
}
