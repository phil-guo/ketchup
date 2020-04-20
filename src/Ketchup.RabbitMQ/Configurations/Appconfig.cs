using Microsoft.Extensions.Configuration;

namespace Ketchup.RabbitMQ.Configurations
{
    public class AppConfig
    {
        public RabbitMqOption RabbitMq { get; set; }

        public AppConfig()
        {
            if (!string.IsNullOrEmpty(RabbitMq.Host))
                GetRabbitMqConfig();
        }

        protected void GetRabbitMqConfig()
        {
            var section = Core.Configurations.AppConfig.GetSection("RabbitMq");

            if (section.Exists())
                RabbitMq = section.Get<RabbitMqOption>();
        }
    }
}
