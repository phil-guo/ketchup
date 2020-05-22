using Microsoft.Extensions.Configuration;

namespace Ketchup.Grpc.Configurations
{
    internal class AppConfig
    {
        public CommandOption Command { get; set; } = new CommandOption();

        public AppConfig()
        {
            GetCommandAppConfig();
        }

        protected CommandOption GetCommandAppConfig()
        {
            var section = Core.Configurations.AppConfig.GetSection("Command");

            if (section.Exists())
                Command = section.Get<CommandOption>();
            return Command;
        }
    }
}
