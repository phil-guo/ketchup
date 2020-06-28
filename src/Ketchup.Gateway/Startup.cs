using System;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ketchup.Core;
using Ketchup.Core.Configurations;
using Ketchup.Core.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Ketchup.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            AppConfig.Configuration = (IConfigurationRoot)configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(option =>
            {
                option.AddPolicy("cors", build => { build.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = false,//是否验证Audience
                        ValidateLifetime = true,//是否验证失效时间
                        ClockSkew = TimeSpan.FromSeconds(7200),
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = "LSyir1XvVA7fJmNjI2Dzxj9sj4JDsakk",//Audience
                        ValidIssuer = "LSyir1XvVA7fJmNjI2Dzxj9sj4JDsakk",//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ktbN7JdBBUIVm4GIy68EYnc6WQ7Zy2h8"))//拿到SecurityKey
                    };
                });

            services.AddControllers().AddNewtonsoftJson();
            services.AddGrpc();

        }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // Add things to the service collection that are only for the
            // development environment.
            services.AddCors(option =>
            {
                option.AddPolicy("cors", build => { build.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateLifetime = true,//是否验证失效时间
                        ClockSkew = TimeSpan.FromSeconds(7200),
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = "LSyir1XvVA7fJmNjI2Dzxj9sj4JDsakk",//Audience
                        ValidIssuer = "LSyir1XvVA7fJmNjI2Dzxj9sj4JDsakk",//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ktbN7JdBBUIVm4GIy68EYnc6WQ7Zy2h8"))//拿到SecurityKey
                    };
                });

            services.AddControllers().AddNewtonsoftJson();
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
            builder.AddCoreService().RegisterModules();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ServiceLocator.Current = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors("cors");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseKetchup();
        }

        public void ConfigureStaging(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Set up the application for staging.
            ServiceLocator.Current = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors("cors");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseKetchup();
        }
    }
}
