using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Autofac;
using Ketchup.Core.Modules;
using Ketchup.Core.Services;
using Microsoft.Extensions.DependencyModel;

namespace Ketchup.Core
{
    public static class ContainerBuilderExtensions
    {
        private static List<Assembly> _referenceAssembly = new List<Assembly>();
        private static List<KernelModule> _modules = new List<KernelModule>();

        public static void RegisterServices(this IServiceBuilder builder)
        {
            try
            {
                var services = builder.Services;
                var referenceAssemblies = GetAssemblies();

                foreach (var assembly in referenceAssemblies)
                {
                    services.RegisterAssemblyTypes(assembly)
                        .Where(t => typeof(IServiceKey).GetTypeInfo().IsAssignableFrom(t) && t.IsInterface)
                        .AsImplementedInterfaces();
                    //services.RegisterAssemblyTypes(assembly)
                    //    .Where(t => typeof(IServiceBehavior).GetTypeInfo().IsAssignableFrom(t) && t.GetTypeInfo().GetCustomAttribute<ModuleNameAttribute>() == null).AsImplementedInterfaces();

                    //var types = assembly.GetTypes().Where(t => typeof(IServiceBehavior).GetTypeInfo().IsAssignableFrom(t) && t.GetTypeInfo().GetCustomAttribute<ModuleNameAttribute>() != null);
                    //foreach (var type in types)
                    //{
                    //    var module = type.GetTypeInfo().GetCustomAttribute<ModuleNameAttribute>();
                    //    var interfaceObj = type.GetInterfaces()
                    //        .FirstOrDefault(t => typeof(IServiceKey).GetTypeInfo().IsAssignableFrom(t));
                    //    if (interfaceObj != null)
                    //    {
                    //        services.RegisterType(type).AsImplementedInterfaces().Named(module.ModuleName, interfaceObj);
                    //        services.RegisterType(type).Named(module.ModuleName, type);
                    //    }
                    //}

                }

            }
            catch (Exception exception)
            {
                if (exception is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = exception as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    throw loaderExceptions[0];
                }
                throw exception;
            }
        }

        private static List<Assembly> GetAssemblies()
        {
            var referenceAssemblies = new List<Assembly>();

            string[] assemblyNames = DependencyContext
                .Default.GetDefaultAssemblyNames().Select(p => p.Name).ToArray();
            assemblyNames = GetFilterAssemblies(assemblyNames);
            foreach (var name in assemblyNames)
                referenceAssemblies.Add(Assembly.Load(name));
            _referenceAssembly.AddRange(referenceAssemblies.Except(_referenceAssembly));

            return referenceAssemblies;
        }

        private static string[] GetFilterAssemblies(string[] assemblyNames)
        {
            var pattern = $"^Microsoft.\\w*|^System.\\w*|^DotNetty.\\w*|^runtime.\\w*|^ZooKeeperNetEx\\w*|^StackExchange.Redis\\w*|^Consul\\w*|^Newtonsoft.Json.\\w*|^Autofac.\\w*";
            Regex notRelatedRegex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return
                assemblyNames.Where(
                    name => !notRelatedRegex.IsMatch(name)).ToArray();
        }
    }

    /// <summary>
    /// 服务构建者。
    /// </summary>
    public interface IServiceBuilder
    {
        /// <summary>
        /// 服务集合。
        /// </summary>
        ContainerBuilder Services { get; set; }
    }
}
