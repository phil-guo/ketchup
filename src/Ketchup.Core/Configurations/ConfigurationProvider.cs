using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ketchup.Core.Configurations.Remote;
using Microsoft.Extensions.Configuration;

namespace Ketchup.Core.Configurations
{
    public class ConfigurationProvider : FileConfigurationProvider
    {
        public ConfigurationProvider(FileConfigurationSource source) : base(source)
        {
        }

        public override void Load(Stream stream)
        {
            var parser = new JsonConfigurationParser();
            this.Data = parser.Parse(stream, null);
        }
    }
}
