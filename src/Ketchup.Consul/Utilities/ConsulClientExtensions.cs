using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Consul;

namespace Ketchup.Consul.Utilities
{
    public static class ConsulClientExtensions
    {
        public static async Task<string[]> GetChildrenAsync(this ConsulClient client, string path)
        {
            try
            {
                var queryResult = await client.KV.List(path);
                return queryResult.Response?.Select(p => Encoding.UTF8.GetString(p.Value)).ToArray();
            }
            catch (HttpRequestException)
            {
                return null;
            }

            AgentServiceRegistration a=new AgentServiceRegistration()
            {
                Check = new AgentServiceCheck()
                {
                    HTTP = 
                }
            };
        }

        public static async Task<byte[]> GetDataAsync(this ConsulClient client, string path)
        {
            try
            {
                var queryResult = await client.KV.Get(path);
                return queryResult.Response?.Value;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
