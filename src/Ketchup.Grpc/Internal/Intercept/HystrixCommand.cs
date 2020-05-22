using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Ketchup.Grpc.Configurations;

namespace Ketchup.Grpc.Internal.Intercept
{
    public class HystrixCommand : Interceptor
    {
        private readonly ICommandProvider provider;

        public HystrixCommand(ICommandProvider provider)
        {
            this.provider = provider;
            var appConfig = new AppConfig();
            Command = appConfig.Command;
        }

        public CommandOption Command { get; }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
            ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (context.Method == "/grpc.health.v1.Health/Check")
                return await continuation(request, context);

            var result = provider.LimitBulkhead(this, context, Command.MaxBulkhead).ValidateWhitelist(context)
                .ValidateBlacklist(context)
                .Execute(async () => await continuation(request, context));

            return result;
        }

        private HystrixCommand ValidateWhitelist(ServerCallContext context)
        {
            if (Command.Whitelist == "*")
                return this;

            var exist = Command.Whitelist.Split("|").Contains(context.Host.Split(":").FirstOrDefault());
            if (exist)
                return this;

            throw new RpcException(new Status(StatusCode.Internal, "ip does not exist in the white list"));
        }

        private HystrixCommand ValidateBlacklist(ServerCallContext context)
        {
            if (string.IsNullOrEmpty(Command.BlackList))
                return this;

            var exist = Command.BlackList.Split("|").Contains(context.Host.Split(":").FirstOrDefault());
            if (!exist)
                return this;
            throw new RpcException(new Status(StatusCode.Internal, "ip exist in the black list"));
        }

        private T Execute<T>(Func<Task<T>> func)
        {
            var manual = new ManualResetEvent(false);
            var result = false;

            var task = Task.Run(async () =>
            {
                var invoke = await func.Invoke();
                result = true;
                manual.Set();
                return invoke;
            });

            manual.WaitOne(Command.TimeOut);

            return result
                ? task.GetAwaiter().GetResult()
                : throw new RpcException(new Status(StatusCode.DeadlineExceeded, "execute timeout"));
        }
    }
}