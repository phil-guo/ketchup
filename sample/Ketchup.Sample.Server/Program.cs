using System.IO;
using System.Net;
using Autofac.Extensions.DependencyInjection;
using Ketchup.Core.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Ketchup.Sample.Server
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
                    if (hostingContext.HostingEnvironment.IsDevelopment())
                        config.AddJsonFile("config/server.json", true, true);
                    if (hostingContext.HostingEnvironment.IsProduction())
                        config.AddJsonFile($"config/server.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                            true);

                }).ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureKestrel(options =>
                        {
                            options.Listen(IPAddress.Any, 5000,
                                listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                        })
                        .UseStartup<Startup>();
                });
        }
    }
}