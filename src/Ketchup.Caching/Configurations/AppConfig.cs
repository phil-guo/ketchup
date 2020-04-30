using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Ketchup.Caching.Configurations
{
    public class AppConfig
    {
        public CacheOption Cache { get; set; }

        public AppConfig()
        {
            GetCacheAppConfig();

            if (string.IsNullOrEmpty(Cache.IpAddress))
                return;
        }

        protected CacheOption GetCacheAppConfig()
        {
            var section = Core.Configurations.AppConfig.GetSection("Consul");

            if (section.Exists())
                Cache = section.Get<CacheOption>();
            return Cache;
        }
    }
}
