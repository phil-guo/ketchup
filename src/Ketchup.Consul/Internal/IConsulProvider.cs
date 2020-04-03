using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ketchup.Consul.Internal
{
    public interface IConsulProvider
    {
        Task RegiserGrpcConsul(string name, string address, int port);
    }
}
