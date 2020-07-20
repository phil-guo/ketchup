using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ketchup.Consul.Internal.ConsulProvider
{
    public interface IServiceRouteProvider
    {
        Task AddCustomerServerRoute();
        Task<string> GetCustomerServerRoute(string key);
    }
}
