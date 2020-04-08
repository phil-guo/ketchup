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

            //var test = new ChannelFactory().FindGrpcClient<RpcTest.RpcTestClient>();

            Console.WriteLine($"{reply.Msg}-----{reply.Code} --------{reply.Result}");
            Console.ReadKey();
        }
    }

    public class ChannelFactory
    {
        public ChannelFactory() { }
        //ClientBase<RpcTestClient>
        //public ClientBase FindGrpcClient<TClient>()
        //    where TClient : ClientBase<TClient>, new()
        //{
        //    AppContext.SetSwitch(
        //        "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        //    //TypeNameHelper.GetTypeDisplayName(typeof(TClient), false, false, true, '+');

        //    var channel = GrpcChannel.ForAddress("http://127.0.0.1:5003", new GrpcChannelOptions() { });

        //    var a = new TClient();

        //    //var a = new RpcTest.RpcTestClient(channel);

        //    //return cliet;

        //    var type = typeof(TClient);

        //    type.Assembly.CreateInstance(nameof(TClient));
        //}
    }
}
