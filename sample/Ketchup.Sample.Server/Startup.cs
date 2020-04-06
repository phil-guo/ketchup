using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ketchup.Consul.Internal.HealthCheck.Implementation;
using Ketchup.Core;
using Ketchup.Core.Configurations;
using Ketchup.Core.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ketchup.Sample.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            AppConfig.Configuration = (IConfigurationRoot)configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add things to the service collection.
        }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // Add things to the service collection that are only for the
            // development environment.
            services.AddGrpc();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Add things to the Autofac ContainerBuilder.
            builder.AddCoreService().RegisterModules();
        }

        public void ConfigureProductionContainer(ContainerBuilder builder)
        {
            // Add things to the ContainerBuilder that are only for the
            // production environment.

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseServer();

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapGrpcService<DefaultHealthCheckService>(); });
        }

        public void ConfigureStaging(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // Set up the application for staging.
        }
    }
}
