using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ketchup.Core.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Ketchup.Core.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
        {
            Check.NotNull(builder, "builder");
            Check.CheckCondition(() => string.IsNullOrEmpty(path), "path");

            path = EnvironmentHelper.GetEnvironmentVariable(path);
            if (File.Exists(path))
            {
                if (provider == null && Path.IsPathRooted(path))
                {
                    provider = new PhysicalFileProvider(Path.GetDirectoryName(path));
                    path = Path.GetFileName(path);
                }
                var source = new ConfigurationSource
                {
                    FileProvider = provider,
                    Path = path,
                    Optional = optional,
                    ReloadOnChange = reloadOnChange
                };

                builder.Add(source);

                AppConfig.Configuration = builder.Build();
                AppConfig.ServerOptions = AppConfig.Configuration.Get<ServerOptions>();
                var section = AppConfig.Configuration.GetSection("Server");
                if (section.Exists())
                    AppConfig.ServerOptions = AppConfig.Configuration.GetSection("Server").Get<ServerOptions>();
            }
            return builder;
        }
    }
}
