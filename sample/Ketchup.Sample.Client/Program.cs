using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Grpc.Domain;
using Grpc.Net.Client;
using Ketchup.Core;
using Ketchup.Core.Configurations;
using Ketchup.Grpc.Internal.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ketchup.Sample.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config/client.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            AppConfig.Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(serviceCollection);

            containerBuilder.AddCoreService().RegisterModules();

            var container = containerBuilder.Build();

            var serviceProvider = new AutofacServiceProvider(container);

            Test.Testing(container.Resolve<IGrpcClientProvider>());
        }
    }

    public class Test
    {
        public static void Testing(IGrpcClientProvider provider)
        {


            Task.Run(async () =>
            {
                var count = 10000;


                //Stopwatch sw1 = new Stopwatch();
                //sw1.Start();
                //Console.WriteLine($"开始执行获取客户端{count}次测试");

                //for (int i = 0; i < count; i++)
                //{
                //    await provider.FindGrpcClient<RpcTest.RpcTestClient>("sample");
                //}


                //sw1.Stop();
                //TimeSpan ts1 = sw1.Elapsed;
                //Console.WriteLine("执行获取客户端总共花费{0}ms.", ts1.TotalMilliseconds);


                var client = await provider.FindGrpcClient<RpcTest.RpcTestClient>("sample");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Console.WriteLine($"开始执行{count}次测试");
                for (int i = 0; i < count; i++)
                {

                    var result = await client.SayHelloAsync(new HelloRequest() { Age = 28, Name = "simple" });
                    //Console.WriteLine($"{result.Msg}========{result.Code}==========={result.Result}");
                }
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                Console.WriteLine("执行总共花费{0}ms.", ts.TotalMilliseconds);

                ////todo 3

                //Stopwatch sw3 = new Stopwatch();
                //sw3.Start();
                //Console.WriteLine($"开始执行{count}次测试");

                //for (int i = 0; i < count; i++)
                //{
                //    var client = await provider.FindGrpcClient<RpcTest.RpcTestClient>("sample");
                //    var result = await client.SayHelloAsync(new HelloRequest() { Age = 28, Name = "simple" });
                //}


                //sw3.Stop();
                //TimeSpan ts3 = sw3.Elapsed;
                //Console.WriteLine("执行获取客户端跟消息通讯总共花费{0}ms.", ts3.TotalMilliseconds);

                Console.ReadKey();
            }).Wait();
        }
    }
}
