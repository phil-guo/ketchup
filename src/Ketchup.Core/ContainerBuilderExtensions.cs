using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Autofac;
using Ketchup.Core.Command;
using Ketchup.Core.Command.Implementation;
using Ketchup.Core.EventBus;
using Ketchup.Core.Kong;
using Ketchup.Core.Kong.Implementation;
using Ketchup.Core.Modules;
using Ketchup.Core.Utilities;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

namespace Ketchup.Core
{
    public static class ContainerBuilderExtensions
    {
        private static readonly List<Assembly> _referenceAssembly = new List<Assembly>();
        private static List<KernelModule> _modules = new List<KernelModule>();

        public static ContainerBuilder AddCoreService(this ContainerBuilder builder)
        {
            GetAssemblies();
            builder.Register(p => new KetchupPlatformContainer(p));
            builder.Register(p => new KetchupPlatformContainer(ServiceLocator.Current));
            builder.RegisterType<CommandProvider>().As<ICommandProvider>()
                .WithParameter(new TypedParameter(typeof(Type[]), _referenceAssembly.SelectMany(i => i.ExportedTypes).ToArray()))
                .SingleInstance();

            builder.RegisterType<KongNetProvider>().As<IKongNetProvider>()
                .WithParameter(new TypedParameter(typeof(Type[]), _referenceAssembly.SelectMany(i => i.ExportedTypes).ToArray()))
                .SingleInstance();

            return builder;
        }

        public static void RegisterModules(this ContainerBuilder builder)
        {
            var referenceAssemblies = GetAssemblies();
            foreach (var moduleAssembly in referenceAssemblies)
            {
                GetKernelModules(moduleAssembly).ForEach(module =>
                {
                    builder.RegisterModule(module);
                    _modules.Add(module);
                });
            }

            builder.Register(provider => new KernelModuleProvider(_modules,
                    provider.Resolve<KetchupPlatformContainer>(),
                    provider.Resolve<ILogger<KernelModuleProvider>>()))
                .As<IKernelModuleProvider>();
        }

        public static ContainerBuilder AddEventBusService(this ContainerBuilder builder)
        {
            var referenceAssemblies = GetAssemblies();
            foreach (var assembly in referenceAssemblies)
            {
                builder.RegisterAssemblyTypes(assembly).Where(t => typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(t)).AsImplementedInterfaces().SingleInstance();
                builder.RegisterAssemblyTypes(assembly).Where(t => typeof(IEventHandler).IsAssignableFrom(t)).SingleInstance();
            }
            return builder;
        }

        private static List<Assembly> GetAssemblies()
        {
            var referenceAssemblies = new List<Assembly>();

            var assemblyNames = DependencyContext
                .Default.GetDefaultAssemblyNames().Select(p => p.Name).ToArray();
            assemblyNames = GetFilterAssemblies(assemblyNames);
            foreach (var name in assemblyNames)
                referenceAssemblies.Add(Assembly.Load(name));
            _referenceAssembly.AddRange(referenceAssemblies.Except(_referenceAssembly));

            return referenceAssemblies;
        }

        private static string[] GetFilterAssemblies(string[] assemblyNames)
        {
            var pattern =
                "^Microsoft.\\w*|^System.\\w*|^DotNetty.\\w*|^runtime.\\w*|^ZooKeeperNetEx\\w*|^StackExchange.Redis\\w*|^Consul\\w*|^Newtonsoft.Json.\\w*|^Autofac.\\w*";
            var notRelatedRegex = new Regex(pattern,
                RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return
                assemblyNames.Where(
                    name => !notRelatedRegex.IsMatch(name)).ToArray();
        }

        private static List<KernelModule> GetKernelModules(Assembly assembly)
        {
            var modules = new List<KernelModule>();
            Type[] arrayModule =
                assembly
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(KernelModule)))
                    .ToArray();

            foreach (var moduleType in arrayModule)
            {
                var abstractModule = (KernelModule)Activator.CreateInstance(moduleType);
                modules.Add(abstractModule);
            }

            return modules;
        }
    }
}