using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ketchup.Consul.Internal.ConsulProvider
{
    public interface IServiceRouteProvider
    {
        Task AddCustomerServerRouter();
        Task<string> GetCustomerServerRouter(string key);
    }
}
