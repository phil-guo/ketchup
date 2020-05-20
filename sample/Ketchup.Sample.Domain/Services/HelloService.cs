using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Domain;
using Ketchup.Caching.Internal;
using Ketchup.Core.EventBus;
using Ketchup.Core.Utilities;
using Ketchup.Sample.Domain.Services.Events;
using Newtonsoft.Json;

namespace Ketchup.Sample.Domain.Services
{
    public class HelloService : RpcTest.RpcTestBase
    {
        private readonly ICacheProvider _cache;

        public HelloService()
        {
            _cache = ServiceLocator.GetService<ICacheProvider>(CacheModel.Redis.ToString());
        }

        public override async Task<HelloReponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            throw new ArgumentNullException();

            var result =await _cache.GetAsync<string>("a");

            return new HelloReponse()
            {
                Code = 1,
                Msg = "hello simple",
                Result = JsonConvert.SerializeObject(request)
            };
        }

        public override Task<HelloReponse> SayHelloEvent(HelloRequest request, ServerCallContext context)
        {
            ServiceLocator.GetService<IEventBus>().Publish(new UserEvent()
            {
                Name = "simple",
                Job = "it"
            });
            return Task.FromResult(new HelloReponse());
        }
    }
}
