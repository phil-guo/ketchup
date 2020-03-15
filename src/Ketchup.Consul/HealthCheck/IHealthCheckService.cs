using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Core.Address;

namespace Ketchup.Consul.HealthCheck
{
    public interface IHealthCheckService
    {
        /// <summary>
        /// 监控
        /// </summary>
        /// <param name="address"></param>
        void Monitor(IpAddressModel address);

        ValueTask<bool> IsHealth(IpAddressModel address);
    }
}
