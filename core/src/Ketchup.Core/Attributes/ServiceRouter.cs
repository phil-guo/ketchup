using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Attributes
{
    public class ServiceRouterAttribute : Attribute
    {
        public string Name { get; set; }
        public string MethodName { get; set; }
        public string Description { get; set; }
    }
}
