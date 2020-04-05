using System.Threading.Tasks;

namespace Ketchup.Consul.Internal.ConsulProvider
{
    public interface IConsulProvider
    {
        Task RegiserGrpcConsul();
    }
}
