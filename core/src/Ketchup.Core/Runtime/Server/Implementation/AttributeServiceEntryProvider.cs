using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ketchup.Core.Runtime.Server.Attributes;
using Microsoft.Extensions.Logging;

namespace Ketchup.Core.Runtime.Server.Implementation
{
    public class AttributeServiceEntryProvider : IServiceEntryProvider
    {
        private readonly IEnumerable<Type> _types;
        private readonly IClrServiceEntryFactory _clrServiceEntryFactory;
        public AttributeServiceEntryProvider(IEnumerable<Type> types,
            IClrServiceEntryFactory clrServiceEntryFactory)
        {
            _types = types;
            _clrServiceEntryFactory = clrServiceEntryFactory;
        }

        public IEnumerable<ServiceEntry> GetEntries()
        {
            var services = GetTypes();
            var entries = new List<ServiceEntry>();
            foreach (var service in services)
            {
                entries.AddRange(_clrServiceEntryFactory.CreateServiceEntry(service));
            }
            return entries;
        }

        public IEnumerable<ServiceEntry> GetALLEntries()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> GetTypes()
        {
            var services = _types.Where(i =>
            {
                var typeInfo = i.GetTypeInfo();
                return typeInfo.IsInterface && typeInfo.GetCustomAttribute<ServiceBundleAttribute>() != null;
            }).Distinct().ToArray();
            return services;
        }
    }
}
