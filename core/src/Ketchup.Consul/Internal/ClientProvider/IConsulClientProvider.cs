using NConsul;

namespace Ketchup.Consul.Internal.ClientProvider
{
    public interface IConsulClientProvider
    {
        ConsulClient GetConsulClient();
    }
}
