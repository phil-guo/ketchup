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
            Stopwatch sw = new Stopwatch();

            Task.Run(async () =>
            {
                var client = await provider.FindGrpcClient<RpcTest.RpcTestClient>("sample");

                sw.Start();
                Console.WriteLine("开始执行1000次测试");
                for (int i = 0; i < 1000; i++)
                {
                    var result = await client.SayHelloAsync(new HelloRequest() { Age = 28, Name = "simple" });
                    //Console.WriteLine($"{result.Msg}========{result.Code}==========={result.Result}");
                }


                sw.Stop();
                TimeSpan ts2 = sw.Elapsed;
                Console.WriteLine("执行总共花费{0}ms.", ts2.TotalMilliseconds);

                Console.ReadKey();
            }).Wait();
        }
    }
}
