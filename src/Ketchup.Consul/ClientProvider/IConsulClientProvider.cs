using System.Collections.Generic;
using System.Threading.Tasks;
using NConsul;

namespace Ketchup.Consul.ClientProvider
{
    public interface IConsulClientProvider
    {
        ConsulClient GetConsulClient();
    }
}
