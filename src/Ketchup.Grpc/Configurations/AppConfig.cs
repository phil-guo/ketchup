using Microsoft.Extensions.Configuration;

namespace Ketchup.Grpc.Configurations
{
    internal class AppConfig
    {
        public SecurityOption Security { get; set; } = new SecurityOption();

        public AppConfig()
        {
            GetCommandAppConfig();
        }

        private SecurityOption GetCommandAppConfig()
        {
            var section = Core.Configurations.AppConfig.GetSection("Server:Security");

            if (section.Exists())
                Security = section.Get<SecurityOption>();
            return Security;
        }
    }
}
