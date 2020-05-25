using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Ketchup.Core.Command.Attributes;

namespace Ketchup.Core.Command.Implementation
{
    public class CommandProvider : ICommandProvider
    {
        private readonly Type[] _types;
        private readonly ConcurrentDictionary<string, int> _maxRequestsDictionary;
        private TimeSpan time;
        public CommandProvider(Type[] types)
        {
            _types = types.Where(type =>
              {
                  var typeInfo = type.GetTypeInfo();
                  return typeInfo.IsClass && typeInfo.GetCustomAttribute<ServiceAttribute>() != null;
              }).Distinct().ToArray();

            _maxRequestsDictionary = new ConcurrentDictionary<string, int>();
            time = new TimeSpan(DateTime.Now.Ticks);
        }

        public bool LimitMaxRequest(ServerCallContext context)
        {
            var date = new TimeSpan(DateTime.Now.Ticks);
            var timeSub = time.Subtract(date).Duration().TotalMilliseconds;
            var command = GetServiceEntryHystrixCommand(context.Method);

            if (command == null)
                return true;

            if (command.MaxRequests == 0)
                return true;

            if (timeSub > command.MaxRequestsTime)
            {
                time = date;
                _maxRequestsDictionary.TryUpdate(context.Method, 1, command.MaxRequests);
                return true;
            }

            var result = _maxRequestsDictionary.TryGetValue(context.Method, out var value);
            if (!result)
            {
                _maxRequestsDictionary.TryAdd(context.Method, 1);
                return true;
            }

            if (value < command.MaxRequests)
            {
                var newValue = value + 1;
                _maxRequestsDictionary.TryUpdate(context.Method, newValue, value);
                return true;
            }

            if (value >= command.MaxRequests)
                return false; //todo 抛弃请求

            return false;
        }

        public T ExecuteTimeout<T>(ServerCallContext context, Func<Task<T>> func)
        {
            var command = GetServiceEntryHystrixCommand(context.Method);
            var manual = new ManualResetEvent(false);
            var result = false;

            var task = Task.Run(async () =>
            {
                var invoke = await func.Invoke();
                result = true;
                manual.Set();
                return invoke;
            });

            manual.WaitOne(command.Timeout);

            return result
                ? task.GetAwaiter().GetResult()
                : throw new RpcException(new Status(StatusCode.DeadlineExceeded, "execute timeout"));
        }

        private HystrixCommandAttribute GetServiceEntryHystrixCommand(string serviceName)
        {
            var service = GetServiceEntry(serviceName);

            if (service == null)
                return null;

            var hystrix = service.GetMethods()
                .FirstOrDefault(item => item.GetCustomAttribute<HystrixCommandAttribute>()?.MethodName == serviceName.Split("/").LastOrDefault())
                ?.GetCustomAttribute<HystrixCommandAttribute>();
            return hystrix;
        }

        private Type GetServiceEntry(string serviceName)
        {
            var name = serviceName.Split("/")[1];
            return _types.FirstOrDefault(item => item.GetTypeInfo().GetCustomAttribute<ServiceAttribute>().Name == name);
        }
    }
}
