using System.Collections.Generic;
using System.Threading.Tasks;
using Consul;

namespace Ketchup.Consul.ClientProvider
{
    public interface IConsulClientProvider
    {
        ConsulClient GetConsulClient();

        ValueTask<ConsulClient> GetClient();

        ValueTask<IEnumerable<ConsulClient>> GetClients();

        ValueTask Check();
    }
}
