using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;

namespace Ketchup.Consul.ClientProvider
{
    public interface IConsulClientProvider
    {
        ValueTask<ConsulClient> GetClient();

        ValueTask<IEnumerable<ConsulClient>> GetClients();

        ValueTask Check();
    }
}
