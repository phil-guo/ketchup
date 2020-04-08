using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ketchup.Consul;
using Ketchup.Consul.Internal.HealthCheck.Implementation;
using Ketchup.Core;
using Ketchup.Core.Configurations;
using Ketchup.Core.Utilities;
using Ketchup.Sample.Domain.Services;
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
            services.AddGrpc();
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
            //builder.Populate(Services);
            builder.AddCoreService().RegisterModules();
        }

        public void ConfigureProductionContainer(ContainerBuilder builder)
        {
            // Add things to the ContainerBuilder that are only for the
            // production environment.
            builder.AddCoreService().RegisterModules();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Set up the application for development.

            ServiceLocator.Current = app.ApplicationServices.GetAutofacRoot();

            app.UseRouting();
            app.UseKetchupServer();
        }

        public void ConfigureStaging(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // Set up the application for staging.
            ServiceLocator.Current = app.ApplicationServices.GetAutofacRoot();
            app.UseRouting();

            app.UseKetchupServer();
        }
    }
}
