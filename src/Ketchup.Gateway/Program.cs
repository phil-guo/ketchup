using System.IO;
using System.Net;
using Autofac.Extensions.DependencyInjection;
using Ketchup.Gateway.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Ketchup.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("config/gateway.json", true, true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureKestrel(options =>
                        {
                            var config = new AppConfig();
                            options.Listen(new IPEndPoint(IPAddress.Any, config.Gateway.Port),
                                listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
                        })
                        .UseStartup<Startup>(); //
                });
        }
    }
}