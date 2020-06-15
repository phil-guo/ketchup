using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Ketchup.Gateway.Internal;
using Ketchup.Gateway.Internal.Filter;
using Ketchup.Gateway.Internal.Implementation;
using Ketchup.Gateway.Model;
using Ketchup.Grpc.Internal.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ketchup.Gateway.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ServicesController : Controller
    {
        private readonly IGrpcClientProvider _clientProvider;
        private readonly IGatewayProvider _gatewayProvider;

        public ServicesController(IGrpcClientProvider clientProvider, IGatewayProvider gatewayProvider)
        {
            this._clientProvider = clientProvider;
            _gatewayProvider = gatewayProvider;
        }

        [HttpPost("{server}/{service}/{method}")]
        [KetchupExceptionFilter]
        public async Task<object> ExecuteService(string server, string service, string method, [FromBody] Dictionary<string, object> inputBody)
        {
            _gatewayProvider.MapClients.TryGetValue($"{service}.{method}", out var value);

            var client = await _clientProvider.GetClientAsync(server, value);

            var descriptor =
                _gatewayProvider.MethodDescriptors.FirstOrDefault(item => item.Name == method);

            var methodModel = new GrpcGatewayMethod()
            {
                Type = descriptor?.InputType.ClrType,
                Request = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(inputBody), descriptor?.InputType.ClrType)
            };

            var methodInvoke = _gatewayProvider.MapClients[$"{service}.{method}"].GetMethod(method,
                new Type[] { methodModel.Type,
                    typeof(Metadata),
                    typeof(global::System.DateTime),
                    typeof(global::System.Threading.CancellationToken) });

            var result = methodInvoke?.Invoke(client,
                new object[] { methodModel.Request, null, null, default(global::System.Threading.CancellationToken) });

            return new KetchupReponse(result);
        }
    }
}
