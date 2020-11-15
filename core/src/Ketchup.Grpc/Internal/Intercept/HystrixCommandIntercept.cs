using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Ketchup.Grpc.Configurations;

namespace Ketchup.Grpc.Internal.Intercept
{
    public class HystrixCommandIntercept : Interceptor
    {
        private readonly Core.Command.ICommandProvider provider;

        public HystrixCommandIntercept(Core.Command.ICommandProvider provider)
        {
            this.provider = provider;
            var appConfig = new AppConfig();
            Security = appConfig.Security;
        }

        public SecurityOption Security { get; }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
            ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (context.Method == "/grpc.health.v1.Health/Check")
                return await continuation(request, context);

            ValidateWhitelist(context).ValidateBlacklist(context);

            if (!provider.LimitMaxRequest(context))
                throw new RpcException(new Status(StatusCode.Aborted, "has reached the maximum current limit"));

            var result = await provider.BreakerRequestCircuitBreaker(context, async () => await continuation(request, context));
            return result;
        }

        private HystrixCommandIntercept ValidateWhitelist(ServerCallContext context)
        {
            if (Security.Whitelist == "*")
                return this;

            var exist = Security.Whitelist.Split("|").Contains(context.Host.Split(":").FirstOrDefault());
            if (exist)
                return this;

            throw new RpcException(new Status(StatusCode.Internal, "ip does not exist in the white list"));
        }

        private HystrixCommandIntercept ValidateBlacklist(ServerCallContext context)
        {
            if (string.IsNullOrEmpty(Security.BlackList))
                return this;

            var exist = Security.BlackList.Split("|").Contains(context.Host.Split(":").FirstOrDefault());
            if (!exist)
                return this;
            throw new RpcException(new Status(StatusCode.Internal, "ip exist in the black list"));
        }
    }
}