using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Ketchup.Core.Serialization.Implementation
{
    public class JsonSerializer : ISerializer<string>
    {
        public string Serialize(object instance)
        {
            return JsonConvert.SerializeObject(instance);
        }

        public object Deserialize(string content, Type type)
        {
            return JsonConvert.DeserializeObject(content, type);
        }
    }
}
