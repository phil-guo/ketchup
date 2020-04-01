using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Core.Configurations;
using Ketchup.Core.Route.Template;
using Ketchup.Core.Runtime.Server.Attributes;

namespace Ketchup.Core.Runtime.Server.Implementation
{
    public class ClrServiceEntryFactory : IClrServiceEntryFactory
    {
        private readonly IServiceIdGenerator _serviceIdGenerator;

        public ClrServiceEntryFactory(IServiceIdGenerator serviceIdGenerator)
        {
            _serviceIdGenerator = serviceIdGenerator;
        }

        public IEnumerable<ServiceEntry> CreateServiceEntry(Type service)
        {
            var routeTemplate = service.GetCustomAttribute<ServiceBundleAttribute>();
            foreach (var methodInfo in service.GetTypeInfo().GetMethods())
            {
                yield return Create(methodInfo, service.Name, routeTemplate.RouteTemplate);
            }
        }


        private ServiceEntry Create(MethodInfo method, string serviceName, string routeTemplate)
        {
            var serviceId = _serviceIdGenerator.GenerateServiceId(method);
            var attributes = method.GetCustomAttributes().ToList();
            var serviceDescriptor = new ServiceDescriptor
            {
                Id = serviceId,
                RoutePath = RoutePatternParser.Parse(routeTemplate, serviceName, method.Name)
            };
            return new ServiceEntry
            {
                Descriptor = serviceDescriptor,
                RoutePath = serviceDescriptor.RoutePath,
                MethodName = method.Name,
                Type = method.DeclaringType,
                Attributes = attributes,
            };
        }
    }
}
