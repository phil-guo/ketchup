using System.Collections.Generic;
using System.Threading.Tasks;
using Ketchup.Consul.ClientProvider.Implementation;
using Ketchup.Consul.Configurations;
using Ketchup.Consul.HealthCheck;
using Ketchup.Consul.Selector;
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
        public async Task GetClientProvider_Test()
        {
            var healthCheckMoq = new Mock<IHealthCheckService>();
            healthCheckMoq.Setup(x => x.IsHealth(It.IsAny<IpAddressModel>()))
                .Returns(() => new ValueTask<bool>(true));

            var addressSelectorMoq = new Mock<IConsulAddressSelector>();
            addressSelectorMoq.Setup(x => x.SelectAsync(It.IsAny<AddressSelectContext>()))
                .Returns(() => new ValueTask<AddressModel>(new IpAddressModel()
                {
                    Ip = "127.0.0.1",
                    Port = 8500
                }));


            var clietnProvider = new ConsulClientProvider(healthCheckMoq.Object,
                addressSelectorMoq.Object, new Mock<ILogger<ConsulClientProvider>>().Object);

            clietnProvider.AppConfig = new AppConfig()
            {
                Addresses = new List<IpAddressModel>()
                {
                    new IpAddressModel()
                    {
                        Ip = "127.0.0.1",
                        Port = 8500
                    }
                },

                Consul = new ConsulOption()
                {
                    ConnectionString = "127.0.0.1:8500"
                }
            };

            var result = await clietnProvider.GetClient();

            Assert.NotNull(result);
        }
    }
}