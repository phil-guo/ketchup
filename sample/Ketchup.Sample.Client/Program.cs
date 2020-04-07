using System;
using System.Threading.Tasks;
using Grpc.Domain;
using Grpc.Net.Client;

namespace Ketchup.Sample.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var channel = GrpcChannel.ForAddress("http://127.0.0.1:5003");
            var client = new RpcTest.RpcTestClient(channel);

            var reply = await client.SayHelloAsync(new HelloRequest() { Name = "2213", Age = 44 });

            Console.WriteLine($"{reply.Msg}-----{reply.Code} --------{reply.Result}");
            Console.ReadKey();
        }
    }
}
