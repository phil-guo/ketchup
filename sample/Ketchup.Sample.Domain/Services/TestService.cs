using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Test;
using Ketchup.Core.Command.Attributes;
using Newtonsoft.Json;

namespace Ketchup.Sample.Domain.Services
{
    [Service(Name = "grpc.test.simpleTest")]
    public class TestService : simpleTest.simpleTestBase
    {
        public override Task<HelloReponse> Test(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new global::Grpc.Test.HelloReponse()
            {
                Code = 1,
                Msg = "hello simple",
                Result = JsonConvert.SerializeObject("")
            });
        }
    }
}
