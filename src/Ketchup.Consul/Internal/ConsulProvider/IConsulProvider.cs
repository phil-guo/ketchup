using System.Threading.Tasks;
using Ketchup.Core.Address;

namespace Ketchup.Consul.Internal.ConsulProvider
{
    public interface IConsulProvider
    {
        Task RegiserConsulAgent();

        ValueTask<IpAddressModel> FindServiceEntry(string serverName);
    }
}
