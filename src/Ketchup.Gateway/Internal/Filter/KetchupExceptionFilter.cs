using System;
using Grpc.Core;
using Ketchup.Gateway.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ketchup.Gateway.Internal.Filter
{
    public class KetchupExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception.InnerException is RpcException rpcException)
                context.Result = new JsonResult(new KetchupReponse(null)
                    {Code = rpcException.Status.StatusCode, Msg = rpcException.Status.Detail});
        }
    }
}