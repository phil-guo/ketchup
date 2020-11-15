using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Extras.Moq;
using Ketchup.Consul.Configurations;
using Ketchup.Consul.Internal.ClientProvider;
using Ketchup.Consul.Internal.ConsulProvider;
using Ketchup.Consul.Internal.ConsulProvider.Implementation;
using Ketchup.Core.Address;
using Ketchup.Core.Configurations;
using Microsoft.Extensions.Options;
using Moq;
using NConsul;
using Xunit;
using Xunit.Abstractions;

namespace Ketchup.Consul.Test
{
    public class ConsulProviderTest
    {
        private ITestOutputHelper _outputHelper;

        public ConsulProviderTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        #region IConuslProvider

        [Fact]
        public async Task RegisterConsulAgent_Test()
        {

            var consulClientProviderMock = new Mock<IConsulClientProvider>();

            consulClientProviderMock
                .Setup(x => x.GetConsulClient())
                .Returns(() => new ConsulClient(
                    config => { config.Address = new Uri("http://127.0.0.1:8500"); },
                    null, h =>
                    {
                        h.UseProxy = false;
                        h.Proxy = null;
                    }));

            using var mock = AutoMock.GetLoose(cfg =>
            {
                cfg.RegisterMock(consulClientProviderMock);
            });
            var consulProvider = mock.Create<DefaultConsulProvider>(
                new TypedParameter(typeof(Ketchup.Consul.Configurations.AppConfig),
                    new Ketchup.Consul.Configurations.AppConfig
                    {
                        Address = new IpAddressModel()
                        {
                            Ip = "127.0.0.1",
                            Port = 8500
                        },
                        Consul = new ConsulOption()
                        {
                            ConnectionString = "127.0.0.1:8500"
                        }
                    }));
            var optionMock = new Mock<IOptions<ServerOptions>>()
                .Setup(x => x.Value)
                .Returns(() => new ServerOptions()
                {
                    Ip = "127.0.0.1",
                    Port = 5006,
                    Name = "ketchup_test"
                });
            await consulProvider.RegisterConsulAgent();
        }


        #endregion
    }
}
