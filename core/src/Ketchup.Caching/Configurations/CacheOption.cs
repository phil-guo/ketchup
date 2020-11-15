using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Caching.Configurations
{
    public class CacheOption
    {
        public string IpAddress { get; set; }

        public string Password { get; set; }

        public int DefaultDatabase { get; set; } = 0;

        public int PoolSize { get; set; } = 50;

        public long WriteBuffer { get; set; } = 10204;

        public string Prefix { get; set; } = "ketchup_";

        /// <summary>
        /// 滑动过期时间
        /// </summary>
        public long SlidingExpiration { get; set; }
    }
}
