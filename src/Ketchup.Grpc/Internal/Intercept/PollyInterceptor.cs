using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Polly;

namespace Ketchup.Grpc.Internal.Intercept
{
    public class PollyInterceptor : Interceptor
    {
        /// <summary>
        ///     重试次数
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        ///     最大并发数
        /// </summary>
        public int MaxBulkhead { get; set; } = 10;

        /// <summary>
        ///     回调方法
        ///     格式：dll名称|方法名称
        /// </summary>
        public string FallbackMethod { get; set; }

        /// <summary>
        ///     超时时间，单位：秒
        /// </summary>
        public int Timeout { get; set; }


        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (context.Method == "/grpc.health.v1.Health/Check")
                return await continuation(request, context);

            try
            {
                return await continuation(request, context);
            }
            catch (Exception e)
            {
                return null;
            }
            //var pollyRetry = Policy<Task<TResponse>>.Handle<Exception>()
            //    .Retry(RetryCount);


            //var pollyBulk = Policy.Bulkhead<Task<TResponse>>(MaxBulkhead);

            //var pollyFallback = Policy<Task<TResponse>>.Handle<Exception>()
            //    .Fallback(item =>
            //    {
            //        return !string.IsNullOrEmpty(FallbackMethod)
            //            ? UseTaskNewMethod<TResponse>(new object[] { request })
            //            : null;
            //    });

            //var timeoutPolly = Policy.Timeout<Task<TResponse>>(30);

            //var policy = Policy.Wrap(pollyFallback, timeoutPolly, pollyBulk);

            //var response = policy.Execute(() =>
            //{
            //    var responseCon = continuation(request, context);
            //    //var call = base.UnaryServerHandler(request, context, continuation);
            //    return responseCon;
            //});
            //return response;

        }


        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var pollyRetry = Policy<AsyncUnaryCall<TResponse>>.Handle<Exception>()
                .Retry(RetryCount);

            var pollyBulk = Policy.Bulkhead<AsyncUnaryCall<TResponse>>(MaxBulkhead);

            var pollyFallback = Policy<AsyncUnaryCall<TResponse>>.Handle<Exception>()
                .Fallback(item =>
                {
                    return !string.IsNullOrEmpty(FallbackMethod)
                        ? UseNewMethod<TResponse>(new object[] { request })
                        : null;
                });

            var timeoutPolly = Policy.Timeout<AsyncUnaryCall<TResponse>>(30);

            var policy = Policy.Wrap(pollyFallback, pollyRetry, timeoutPolly, pollyBulk);

            var response = policy.Execute(() =>
            {
                var responseCon = continuation(request, context);
                var call = new AsyncUnaryCall<TResponse>(responseCon.ResponseAsync,
                    responseCon.ResponseHeadersAsync,
                    responseCon.GetStatus,
                    responseCon.GetTrailers,
                    responseCon.Dispose);

                return call;
            });
            return response;
        }

        private AsyncUnaryCall<TResponse> UseNewMethod<TResponse>(object[] parameters = null)
            where TResponse : class
        {
            var assemblyArrary = FallbackMethod.Split("|");
            var assembly = Assembly.Load(assemblyArrary?.FirstOrDefault());

            var instance = Activator.CreateInstance(assembly.GetType());
            var obj = assembly?.GetType().GetMethod(assemblyArrary.LastOrDefault()).Invoke(instance, parameters);

            return obj as AsyncUnaryCall<TResponse>;
        }

        private Task<TResponse> UseTaskNewMethod<TResponse>(object[] parameters = null)
            where TResponse : class
        {
            var assemblyArrary = FallbackMethod.Split("|");
            var assembly = Assembly.Load(assemblyArrary?.FirstOrDefault());

            var instance = Activator.CreateInstance(assembly.GetType());
            var obj = assembly?.GetType().GetMethod(assemblyArrary.LastOrDefault()).Invoke(instance, parameters);

            return obj as Task<TResponse>;
        }
    }
}