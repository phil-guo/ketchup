using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ketchup.Core.Configurations.Remote
{
    public interface IConfigurationParser
    {
        IDictionary<string, string> Parse(Stream input, string initialContext);
    }
}
