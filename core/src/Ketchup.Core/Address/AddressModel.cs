using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Ketchup.Core.Address
{
    public abstract class AddressModel
    {
        public abstract EndPoint CreateEndPoint();
    }
}
