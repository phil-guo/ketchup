using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ketchup.Consul.Internal.ConsulProvider;
using Ketchup.Gateway.Internal;
using Ketchup.Gateway.Internal.Filter;
using Ketchup.Gateway.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ketchup.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    public class ServersController : Controller
    {
        private readonly IServerProvider _server;
        private readonly IGatewayProvider _gatewayProvider;

        public ServersController(IServerProvider server, IGatewayProvider gatewayProvider)
        {
            _server = server;
            _gatewayProvider = gatewayProvider;
        }

        [HttpGet("api/servers/getAllServer")]
        [KetchupExceptionFilter]
        public async Task<object> GetAllServer()
        {
            return new KetchupResponse(await _server.GetAllServer());
        }

        [HttpGet("api/servers/getAllServerEntry")]
        [KetchupExceptionFilter]
        public async Task<object> GetAllServerEntry(string server, string service)
        {
            var entries = await _server.GetAllServerEntry(server, service);
            foreach (var item in entries)
            {
                var entry = _gatewayProvider.MethodDescriptors.FirstOrDefault(
                    method => method.FullName.ToLower() == item.FullName.ToLower());
                if (entry == null)
                    continue;

                var dictionary = new Dictionary<string, string>();

                entry.InputType.ClrType.GetProperties().ToList().ForEach(property =>
                {
                    if (property.Name == "Descriptor" || property.Name == "Parser")
                        return;
                    dictionary.Add(property.Name?.Substring(0, 1).ToLower() + property.Name?.Substring(1),
                        property.PropertyType.ToString());
                });

                item.Parameter = JsonConvert.SerializeObject(dictionary);
            }

            return new KetchupResponse(entries);
        }
    }
}
