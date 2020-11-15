using System;
using System.Collections.Generic;

namespace Kong.Models
{
    public class PluginCollection
    {
        public List<Plugin> Data { get; set; }
        public string Next { get; set; }
    }

    public class Plugin
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public DateTime? Created_at { get; set; }
        public RouteId Route { get; set; }
        public class RouteId
        {
            public Guid Id { get; set; }
        }
        public ServiceId Service { get; set; }

        public class ServiceId
        {
            public Guid Id { get; set; }
        }
        public ConsumerId Consumer { get; set; }

        public class ConsumerId
        {
            public Guid Id { get; set; }
        }
        public JwtPluginConfig Config { get; set; }
        public string[] Protocols { get; set; }
        public bool Enabled { get; set; }
        public string[] Tags { get; set; }
    }

    public class JwtPluginConfig
    {
        public string[] uri_param_names { get; set; }
        public string[] cookie_names { get; set; }
        public string[] header_names { get; set; } = new[] { "Authorization" };
        public string claims_to_verify { get; set; }
        public string key_claim_name { get; set; } = "iss";
        public bool secret_is_base64 { get; set; }
        public string anonymous { get; set; }
        public bool run_on_preflight { get; set; } = true;
        public int maximum_expiration { get; set; }
    }

    public class PluginConfig
    {
        public int Minute { get; set; }
        public int Hour { get; set; }
    }

    public class PluginEnabled
    {
        public string[] Enabled_plugins { get; set; }
    }

    public class PluginSchema
    {
        public System.Text.Json.JsonDocument Fields { get; set; }
    }
}
