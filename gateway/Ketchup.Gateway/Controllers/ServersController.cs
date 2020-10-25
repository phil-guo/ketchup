using System.Threading.Tasks;
using Ketchup.Consul.Internal.ConsulProvider;
using Ketchup.Gateway.Internal.Filter;
using Ketchup.Gateway.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ketchup.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    public class ServersController : Controller
    {
        private readonly IServerProvider _server;

        public ServersController(IServerProvider server)
        {
            _server = server;
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
            return new KetchupResponse(await _server.GetAllServerEntry(server, service));
        }
    }
}
