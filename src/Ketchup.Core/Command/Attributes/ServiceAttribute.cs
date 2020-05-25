using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Command.Attributes
{
    public class ServiceAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
