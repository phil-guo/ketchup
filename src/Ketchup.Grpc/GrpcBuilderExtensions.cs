using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Polly;

namespace Ketchup.Grpc
{
    public static class GrpcBuilderExtensions
    {
        /// <summary>
        /// 服务治理策略
        /// </summary>
        /// <typeparam name="TClient">客户端扩展方法</typeparam>
        /// <typeparam name="TRequest">请求参数</typeparam>
        /// <typeparam name="TResponse">响应数据</typeparam>
        /// <param name="action">执行调用委托</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="maxBulkhead">最大并发数</param>
        /// <param name="timeout">超时数</param>
        /// <param name="fallbackMethod">降级方法。默认返回null</param>
        /// <returns>响应数据</returns>
        public static Task<TResponse> AddPollyExecuteAsync<TClient, TRequest, TResponse>(this TClient client, TRequest request, Func<Task<TResponse>> action, int retryCount = 3, int maxBulkhead = 10, int timeout = 30, string fallbackMethod = "")
            where TClient : ClientBase<TClient>
            where TResponse : class, new()
            where TRequest : class
        {
            var pollyRetry = Policy<TResponse>.Handle<Exception>()
                .RetryAsync(retryCount);

            var pollyBulk = Policy.BulkheadAsync<TResponse>(maxBulkhead);

            var pollyFallback = Policy<TResponse>.Handle<Exception>()
                .FallbackAsync(async item =>
                {
                    return !string.IsNullOrEmpty(fallbackMethod)
                        ? await UseNewMethod<TResponse>(fallbackMethod, new object[] { request })
                        : null;
                });

            var timeoutPolly = Policy.TimeoutAsync<TResponse>(timeout);

            var policy = Policy.WrapAsync(pollyFallback, pollyRetry, timeoutPolly, pollyBulk);

            return policy.ExecuteAsync(action);
        }

        private static Task<TResponse> UseNewMethod<TResponse>(string fallbackMethod, object[] parameters = null)
            where TResponse : class
        {
            var assemblyArrary = fallbackMethod.Split("|");
            var assembly = Assembly.Load(assemblyArrary?.FirstOrDefault());

            var instance = Activator.CreateInstance(assembly.GetType());
            var obj = assembly?.GetType().GetMethod(assemblyArrary.LastOrDefault()).Invoke(instance, parameters);

            return Task.FromResult(obj as TResponse);
        }
    }
}
