using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ketchup.Gateway.Internal.Implementation
{
    public class GrpcGatewayMethod
    {
        internal object Request { get; set; }

        internal Type Type { get; set; }
    }
}
