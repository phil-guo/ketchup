using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Ketchup.Consul.Internal.ClientProvider;
using Ketchup.Consul.Internal.ConsulProvider.Implementation;
using Moq;
using NConsul;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Ketchup.Consul.Test
{
    public class ServerProviderTest
    {
        private readonly ITestOutputHelper _outputHelper;

        public ServerProviderTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "获取指定目录下的服务条目")]
        public async Task GetAllServerEntry_Test()
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

            var serviceRouter = mock.Create<ServerProvider>();
            var one = await serviceRouter.GetAllServerEntry("zero", "");
            _outputHelper.WriteLine(JsonConvert.SerializeObject(one));
        }

        [Fact(DisplayName = "获取所有的服务节点")]
        public async Task GetAllServerRouter_Test()
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

            var serviceRouter = mock.Create<ServerProvider>();
            var one = await serviceRouter.GetAllServer();

            _outputHelper.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(one));

            Assert.Equal(1, one.Count);
        }
    }
}
