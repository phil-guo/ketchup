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

        /// <summary>
        /// 负载分流策略
        /// </summary>
        public string Strategy { get; set; } = "Random";
    }
}
