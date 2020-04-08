using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Domain;
using Newtonsoft.Json;

namespace Ketchup.Sample.Domain.Services
{
    public class HelloService : RpcTest.RpcTestBase
    {
        public override Task<HelloReponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReponse()
            {
                Code = 1,
                Msg = "hello simple",
                Result = JsonConvert.SerializeObject(request)
            });
        }
    }
}
