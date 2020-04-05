using Microsoft.Extensions.Configuration;

namespace Ketchup.Core.Configurations
{
    public class AppConfig
    {
        public static IConfigurationRoot Configuration { get; set; }

        public AppConfig()
        {
            //ServerOptions = Configuration.Get<ServerOptions>();
        }

        public static IConfigurationSection GetSection(string name)
        {
            return Configuration?.GetSection(name);
        }

        public static ServerOptions ServerOptions => Configuration.Get<ServerOptions>();
    }
}
