using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Ketchup.Core.Address
{
    public sealed class IpAddressModel : AddressModel
    {
        public string Ip { get; set; }

        public int Port { get; set; }

        public IDictionary<string,string> Meta { get; set; }

        public override EndPoint CreateEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(Ip), Port);
        }
    }
}
