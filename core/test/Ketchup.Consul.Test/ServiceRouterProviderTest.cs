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
using Newtonsoft.Json.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Ketchup.Consul.Test
{
    public class ServiceRouterProviderTest
    {
        private ITestOutputHelper _outputHelper;

        public ServiceRouterProviderTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        
    }
}
