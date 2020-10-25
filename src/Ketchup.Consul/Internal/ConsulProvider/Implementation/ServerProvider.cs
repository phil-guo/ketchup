using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Consul.Internal.ClientProvider;
using Ketchup.Consul.Internal.ConsulProvider.Model;

namespace Ketchup.Consul.Internal.ConsulProvider.Implementation
{
    public class ServerProvider: IServerProvider
    {
        private readonly IConsulClientProvider _client;

        public ServerProvider(IConsulClientProvider client)
        {
            _client = client;
        }

        public void GetAllServerEntry(string server)
        {

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
