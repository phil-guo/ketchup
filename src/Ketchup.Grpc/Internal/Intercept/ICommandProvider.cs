using Grpc.Core;

namespace Ketchup.Grpc.Internal.Intercept
{
    public interface ICommandProvider
    {
        HystrixCommand LimitBulkhead(HystrixCommand command, ServerCallContext context, int limitCount);
    }
}
