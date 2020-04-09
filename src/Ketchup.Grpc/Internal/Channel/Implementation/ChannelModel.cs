using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Net.Client;

namespace Ketchup.Grpc.Internal.Channel.Implementation
{
    public class ChannelModel
    {
        public  bool IsUse { get; set; }
        public DateTime LastTime { get; set; }
        public GrpcChannel Channel { get; set; }
    }
}
