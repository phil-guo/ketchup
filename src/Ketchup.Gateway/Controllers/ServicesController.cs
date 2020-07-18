using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Ketchup.Consul.Internal.ConsulProvider;
using Ketchup.Gateway.Configurations;
using Ketchup.Gateway.Internal;
using Ketchup.Gateway.Internal.Filter;
using Ketchup.Gateway.Internal.Implementation;
using Ketchup.Gateway.Model;
using Ketchup.Grpc.Internal.Client;
using Ketchup.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Ketchup.Gateway.Controllers
{
    [ApiController]
    public class ServicesController : Controller
    {
        private readonly IGrpcClientProvider _clientProvider;
        private readonly IGatewayProvider _gatewayProvider;
        private readonly IServiceRouteProvider _routeProvider;

        public ServicesController(IGrpcClientProvider clientProvider, IGatewayProvider gatewayProvider, IServiceRouteProvider routeProvider)
        {
            this._clientProvider = clientProvider;
            _gatewayProvider = gatewayProvider;
            _routeProvider = routeProvider;
        }

        [HttpPost("auth/token")]
        [KetchupExceptionFilter]
        public async Task<object> GetToken(TokenRequst request)
        {
            var client = await _clientProvider.FindGrpcClient<Auth.AuthClient>("zero");
            var user = await client.LoginAsync(request);

            var config = new AppConfig();
            var token = new JwtSecurityToken(
                   audience: config.Gateway.Key,
                   claims: new[]
                   {
                    new Claim(ClaimTypes.Sid, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                   },
                   issuer: config.Gateway.Key,
                   notBefore: DateTime.Now,
                   expires: DateTime.Now.AddSeconds(7200),
                   signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Gateway.Secret)), SecurityAlgorithms.HmacSha256)
               );

            return new KetchupResponse(new
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = user.UserId,
                UserName = user.UserName,
                Expired = config.Gateway.AuthExpired,
                RoleId = user.RoleId
            });
        }

        [HttpPost("api/{server}/{service}/{method}")]
        [KetchupExceptionFilter]
        [Authorize]
        public async Task<object> ExecuteService(string server, string service, string method, [FromBody] Dictionary<string, object> inputBody)
        {
            var clientType = await GetClientType($"{service}.{method}");

            var client = await _clientProvider.GetClientAsync(server, clientType);

            var descriptor =
                _gatewayProvider.MethodDescriptors.FirstOrDefault(item => item.Name == method);

            var methodModel = new GrpcGatewayMethod()
            {
                Type = descriptor?.InputType.ClrType,
                Request = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(inputBody), descriptor?.InputType.ClrType)
            };

            var methodInvoke = clientType.GetMethod(method,
                new Type[] { methodModel.Type,
                    typeof(Metadata),
                    typeof(global::System.DateTime),
                    typeof(global::System.Threading.CancellationToken) });


            var result = methodInvoke?.Invoke(client,
                new object[] { methodModel.Request, null, null, default(global::System.Threading.CancellationToken) });

            return new KetchupResponse(result);
        }

        private async Task<Type> GetClientType(string key)
        {
            _gatewayProvider.MapClients.TryGetValue(key, out var value);
            if (value != null)
                return value;

            var clientType = Type.GetType(await _routeProvider.GetCustomerServerRoute(key));
            _gatewayProvider.MapClients.TryAdd(key, clientType);

            return clientType;
        }
    }
}
