using System;

namespace Ketchup.Core.Attributes
{
    public class ServiceAttribute : Attribute
    {
        public string Name { get; set; }
        public string TypeClientName { get; set; }
        public string Package { get; set; }
    }
}
