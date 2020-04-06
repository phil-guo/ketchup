using Microsoft.Extensions.Configuration;

namespace Ketchup.Core.Configurations
{
    public class AppConfig
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static IConfigurationSection GetSection(string name)
        {
            return Configuration?.GetSection(name);
        }

        public static ServerOptions ServerOptions
        {
            get
            {
                var section = GetSection("Server");
                return section.Exists() ? section.Get<ServerOptions>() : new ServerOptions();
            }
        }
    }
}
