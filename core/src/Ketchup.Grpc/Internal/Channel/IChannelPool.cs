using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Net.Client;
using Ketchup.Core.Address;

namespace Ketchup.Grpc.Internal.Channel
{
    public interface IChannelPool
    {
        GrpcChannel GetOrAddChannelPool(IpAddressModel address, GrpcChannelOptions options);
    }
}
