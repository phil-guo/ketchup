using Microsoft.Extensions.Configuration;

namespace Ketchup.Core.Configurations
{
    public class AppConfig
    {
        internal static IConfigurationRoot Configuration { get; set; }

        public static IConfigurationSection GetSection(string name)
        {
            return Configuration?.GetSection(name);
        }

        public static ServerOptions ServerOptions { get; internal set; } = new ServerOptions();
    }
}
