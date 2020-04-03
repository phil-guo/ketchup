using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Ketchup.Core.Route;
using Ketchup.Core.Runtime.Server.Implementation;
using Xunit;

namespace Ketchup.Core.Test
{
    public class RoutingServicesTest
    {
        private readonly AutoMock _autoMock;

        public RoutingServicesTest()
        {
            _autoMock = AutoMock.GetLoose();
        }

        [Fact]
        public void GetEntries_Test()
        {
            var moq = _autoMock.Mock<IServiceRouteProvider>();

            var moqInstance = _autoMock.Create<AttributeServiceEntryProvider>();
            var result = moqInstance.GetEntries();
        }
    }
}
