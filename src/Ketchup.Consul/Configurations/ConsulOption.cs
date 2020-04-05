using Ketchup.Core.Address;

namespace Ketchup.Consul.Configurations
{
    public class ConsulOption
    {
        public bool IsHealthCheck { get; set; } = false;

        /// <summary>
        /// consul 链接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        ///// <summary>
        ///// 服务地址
        ///// </summary>
        //public string ServicePath { get; set; } = "services/serviceRoutes/";
    }
}
