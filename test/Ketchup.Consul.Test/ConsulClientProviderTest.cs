using System.Collections.Generic;
using System.Threading.Tasks;
using Ketchup.Consul.Configurations;
using Ketchup.Consul.Internal.ClientProvider.Implementation;
using Ketchup.Consul.Internal.Selector;
using Ketchup.Core.Address;
using Ketchup.Core.Address.Selectors.Implementation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ketchup.Consul.Test
{
    public class ConsulClientProviderTest
    {
        [Fact]
        public void GetConsulClient_Test()
        {
            var clientProvider = new ConsulClientProvider
            {
                AppConfig = new AppConfig()
                {
                    Address = new IpAddressModel { Ip = "127.0.0.1", Port = 8500 },
                    Consul = new ConsulOption() { ConnectionString = "127.0.0.1:8500" }
                }
            };


            var one = clientProvider.GetConsulClient();

            Assert.NotNull(one);

            //todo 获取1000次
            for (int i = 0; i < 1000; i++)
            {
                var two = clientProvider.GetConsulClient();
                Assert.NotNull(two);
            }

            //var healthCheckMoq = new Mock<IHealthCheckService>();
            //healthCheckMoq.Setup(x => x.IsHealth(It.IsAny<IpAddressModel>()))
            //    .Returns(() => new ValueTask<bool>(true));

            //var addressSelectorMoq = new Mock<IConsulAddressSelector>();
            //addressSelectorMoq.Setup(x => x.SelectAsync(It.IsAny<AddressSelectContext>()))
            //    .Returns(() => new ValueTask<AddressModel>(new IpAddressModel()
            //    {
            //        Ip = "127.0.0.1",
            //        Port = 8500
            //    }));
        }
    }
}