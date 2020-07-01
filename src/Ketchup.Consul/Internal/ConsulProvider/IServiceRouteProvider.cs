using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ketchup.Consul.Internal.ConsulProvider
{
    public interface IServiceRouteProvider
    {
        Task AddCustumerServerRoute();
        Task<string> GetCustumerServerRoute(string key);
    }
}
