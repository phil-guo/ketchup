using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;

namespace Ketchup.Core.Command
{
    public interface ICommandProvider
    {
        bool LimitMaxRequest(ServerCallContext context);

        T ExecuteTimeout<T>(ServerCallContext context, Func<Task<T>> func);
    }
}
