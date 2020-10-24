using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ketchup.Consul.Internal.ConsulProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ketchup.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    public class ServersController : Controller
    {
        private readonly IServiceRouteProvider _routeProvider;

        public ServersController(IServiceRouteProvider routeProvider)
        {
            _routeProvider = routeProvider;
        }

        public void GetServices()
        {

        }
    }
}
