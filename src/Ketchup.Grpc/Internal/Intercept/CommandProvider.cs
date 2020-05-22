using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Ketchup.Grpc.Internal.Intercept
{
    public class CommandProvider : ICommandProvider
    {
        public ConcurrentDictionary<string, int> _maxBulkheadDictionary;

        private TimeSpan time;

        public CommandProvider()
        {
            _maxBulkheadDictionary = new ConcurrentDictionary<string, int>();
            time = new TimeSpan(DateTime.Now.Ticks);
        }

        public HystrixCommand LimitBulkhead(HystrixCommand command, ServerCallContext context, int limitCount)
        {
            //todo 计数器算法
            var date = new TimeSpan(DateTime.Now.Ticks);

            double timeSub = time.Subtract(date).Duration().TotalMilliseconds;

            if (limitCount == 0)
                return command;

            //1s内的最大请求数
            if (timeSub < 1000)
            {
                var result = _maxBulkheadDictionary.TryGetValue(context.Method, out var value);

                if (!result)
                {
                    _maxBulkheadDictionary.TryAdd(context.Method, 1);
                    return command;
                }

                if (value < limitCount)
                {
                    var newValue = value + 1;
                    _maxBulkheadDictionary.TryUpdate(context.Method, newValue, value);
                    return command;
                }

                if (value >= limitCount)
                {
                    //_maxBulkheadDictionary.TryRemove(context.Method, out var v);
                    //todo 抛弃请求
                }
            }
            else
            {
                time = date;
                _maxBulkheadDictionary.TryRemove(context.Method, out var v);
            }
            return command;
        }
    }
}
