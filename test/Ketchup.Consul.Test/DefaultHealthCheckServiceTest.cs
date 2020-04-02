using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Consul.HealthCheck.Implementation;
using Ketchup.Core.Address;
using Xunit;

namespace Ketchup.Consul.Test
{
    public class DefaultHealthCheckServiceTest
    {
        [Fact]
        public void Monitor_Test()
        {
            //var moq = new DefaultHealthCheckService();
            //moq.Monitor(new IpAddressModel()
            //{
            //    Ip = "127.0.0.1",
            //    Port = 8500
            //});
        }

        [Fact]
        public async Task IsHealth_Test()
        {
            //var moq = new DefaultHealthCheckService();
            //var ipModel = new IpAddressModel()
            //{
            //    Ip = "127.0.0.1",
            //    Port = 8500
            //};

            //moq.Monitor(ipModel);

            //var result = await moq.IsHealth(ipModel);

            //Assert.False(result);
        }
    }
}
