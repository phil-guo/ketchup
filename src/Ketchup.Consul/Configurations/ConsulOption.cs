namespace Ketchup.Consul.Configurations
{
    public class ConsulOption
    {
        public string Ip { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// consul 链接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 服务地址
        /// </summary>
        public string ServicePath { get; set; } = "services/serviceRoutes/";

        /// <summary>
        /// 缓存地址
        /// </summary>
        public string CachePath { get; set; } = "services/serviceCachesRoutes/";

        /// <summary>
        /// watch时间
        /// </summary>
        public int Watch { get; set; } = 60;
    }
}
