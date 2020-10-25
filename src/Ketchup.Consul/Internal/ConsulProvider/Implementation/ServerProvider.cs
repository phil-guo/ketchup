using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Consul.Internal.ClientProvider;
using Ketchup.Consul.Internal.ConsulProvider.Model;
using Newtonsoft.Json;

namespace Ketchup.Consul.Internal.ConsulProvider.Implementation
{
    public class ServerProvider : IServerProvider
    {
        private readonly IConsulClientProvider _client;
        private const string ENTRY_PREFIX = "serviceRouters";

        public ServerProvider(IConsulClientProvider client)
        {
            _client = client;
        }

        public async Task<List<ServerRouterModel>> GetAllServerEntry(string server, string service)
        {
            var consul = _client.GetConsulClient();
            var models = new List<ServerRouterModel>();

            var entries = await consul.KV.List($"{ENTRY_PREFIX}/{server}/{service}");
            entries.Response.ToList().ForEach(item =>
            {
                var value = Encoding.UTF8.GetString(item.Value);
                var api = item.Key.Replace(ENTRY_PREFIX, "api");
                var method = api.Split('/').LastOrDefault();
                var newMethod = method?.Substring(0, 1).ToLower() + method?.Substring(1);
                var model = JsonConvert.DeserializeObject<ServerRouterModel>(value);
                model.ApiUrl = api.Replace(api.Split('/').LastOrDefault() ?? string.Empty, newMethod);
                models.Add(model);
            });

            return models;
        }


        public async Task<List<ServerModel>> GetAllServer()
        {
            var consulClient = _client.GetConsulClient();
            var servers = (await consulClient.Agent.Services()).Response;

            var models = new List<ServerModel>();

            foreach (var server in servers)
            {
                if (!models.Exists(item => item.Name == server.Value.Service))
                    models.Add(new ServerModel()
                    {
                        Name = server.Value.Service,
                        Cluster = new List<ServerClusterModel>()
                        {
                            new ServerClusterModel()
                            {
                                Ip = server.Value.Address,
                                Port = server.Value.Port.ToString(),
                                Key = server.Key,
                            }
                        }
                    });

                else
                {
                    var model = models.FirstOrDefault(item => item.Name == server.Value.Service);
                    model?.Cluster.Add(new ServerClusterModel()
                    {
                        Ip = server.Value.Address,
                        Port = server.Value.Port.ToString(),
                        Key = server.Key
                    });
                }
            }

            return models;
        }
    }
}
