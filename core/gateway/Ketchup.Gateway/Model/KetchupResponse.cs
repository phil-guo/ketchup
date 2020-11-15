using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;

namespace Ketchup.Gateway.Model
{
    public class KetchupResponse
    {
        public string Msg { get; set; } = "ok";
        public object Result { get; set; }
        public StatusCode Code { get; set; } = StatusCode.OK;

        public KetchupResponse() { }

        public KetchupResponse(object result)
        {
            Result = result;
        }
    }
}
