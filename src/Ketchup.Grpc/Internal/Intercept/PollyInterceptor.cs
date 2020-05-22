using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Polly;
using Polly.NoOp;

namespace Ketchup.Grpc.Internal.Intercept
{
    public class PollyInterceptor : Interceptor
    {
        /// <summary>
        ///     重试次数
        /// </summary>
        public int RetryCount { get; set; } = 2;

        /// <summary>
        ///     最大并发数
        /// </summary>
        public int MaxBulkhead { get; set; } = 0;

        /// <summary>
        ///     回调方法
        ///     格式：dll名称|方法名称
        /// </summary>
        public string FallbackMethod { get; set; }

        /// <summary>
        ///     超时时间，单位：秒
        /// </summary>
        public int Timeout { get; set; }

        AsyncNoOpPolicy policyAsync = Policy.NoOpAsync();


        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (context.Method == "/grpc.health.v1.Health/Check")
                return await continuation(request, context);



            if (RetryCount > 0)
            {
                var pollyRetryAsync = Policy<TResponse>.Handle<RpcException>(ex => ex.Status.StatusCode == StatusCode.Unknown)
                    .RetryAsync(RetryCount);

                policyAsync.WrapAsync(pollyRetryAsync);
            }

            if (MaxBulkhead > 0)
            {
                var pollyBulkAsync = Policy.BulkheadAsync<TResponse>(MaxBulkhead);
                policyAsync.WrapAsync(pollyBulkAsync);
            }

            if (Timeout > 0)
            {
                var timeoutPollyAsync = Policy.TimeoutAsync<TResponse>(Timeout);
                policyAsync.WrapAsync(timeoutPollyAsync);
            }

            //var pollyFallbackAsync = Policy<TResponse>.Handle<Exception>()
            //    .FallbackAsync(async item =>
            //    {
            //        return !string.IsNullOrEmpty(FallbackMethod)
            //            ? await UseTaskNewMethod<TResponse>(new object[] { request })
            //            : Activator.CreateInstance(typeof(TResponse)) as TResponse;
            //    });

            //policyAsync.WrapAsync(pollyFallbackAsync);

            try
            {
                var pollyRetryAsync1 = Policy<TResponse>.Handle<RpcException>(ex => ex.Status.StatusCode == StatusCode.Unknown)
                    .RetryAsync();

                var result = await pollyRetryAsync1.ExecuteAsync(async () => await continuation(request, context));

                return result;
            }
            catch (Exception e)
            {
                return Activator.CreateInstance(typeof(TResponse)) as TResponse;
            }



            //try
            //{
            //    var result = await continuation(request, context);
            //    return result;
            //}
            //catch (Exception e)
            //{
            //    return Activator.CreateInstance(typeof(TResponse)) as TResponse;
            //}
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                return base.AsyncUnaryCall(request, context, continuation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var pollyRetry = Policy<AsyncUnaryCall<TResponse>>.Handle<RpcException>(ex => ex.StatusCode == StatusCode.Unknown)
                .Retry(RetryCount, (exception, retryCount, context) =>
                {
                    Console.WriteLine("执行次数:" + retryCount);
                });

            //var pollyBulk = Policy.Bulkhead<AsyncUnaryCall<TResponse>>(MaxBulkhead);

            //var pollyFallback = Policy<AsyncUnaryCall<TResponse>>.Handle<Exception>()
            //    .Fallback(item =>
            //    {
            //        return !string.IsNullOrEmpty(FallbackMethod)
            //            ? UseNewMethod<TResponse>(new object[] { request })
            //            : null;
            //    });

            var timeoutPolly = Policy.Timeout<AsyncUnaryCall<TResponse>>(30);

            //var policy = Policy.Wrap(/*pollyFallback,*/ pollyRetry, timeoutPolly/*, pollyBulk*/);

            var response = pollyRetry.Execute(() =>
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

            return Task.FromResult(obj as TResponse);
        }
    }
}