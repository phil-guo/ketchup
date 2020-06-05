using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Domain;
using Ketchup.Core.Utilities;
using Ketchup.Grpc.Internal.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ketchup.Gateway.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ServicesController : ControllerBase
    {
        private readonly IGrpcClientProvider _clientProvider;

        public ServicesController(IGrpcClientProvider clientProvider)
        {
            this._clientProvider = clientProvider;
        }

        [HttpGet("getService")]
        public async Task<object> GetService(string path)
        {
            var serverName = HttpContext.Request.Query;

            var client = await _clientProvider.FindGrpcClient<RpcTest.RpcTestClient>("sample");

            var request = new HelloRequest() { Age = 28, Name = "simple" };

            //await client.SayHelloAsync(request);

            var method = client.GetType().GetMethods().Where(item => item.Name == "SayHello")?.LastOrDefault();

            //var invoke = FastInvoke.GetMethodInvoker(method);

            //await (Task)invoke(client.GetType(), new[] { request });

            var result = method?.Invoke(client.GetType(), new object[] { request });

            return Task.FromResult("ok");
        }

        [HttpPost("postService")]
        public async Task<object> PostService(string path, [FromBody]Dictionary<string, object> inputBody)
        {
            return Task.FromResult("ok");
        }
    }
}
