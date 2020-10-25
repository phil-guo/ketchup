using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Consul.Internal.ConsulProvider.Model;

namespace Ketchup.Consul.Internal.ConsulProvider
{
    public interface IServerProvider
    {
        Task<List<ServerModel>> GetAllServer();
        Task<List<ServerRouterModel>> GetAllServerEntry(string server, string service);
    }
}
