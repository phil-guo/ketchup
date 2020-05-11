using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using Ketchup.Caching.Configurations;
using Ketchup.Caching.Internal;
using Ketchup.Caching.Internal.Redis;
using Xunit;

namespace Ketchup.Redis.Test
{
    public class RedisProviderTest
    {
        private readonly AutoMock _autoMock;

        public RedisProviderTest()
        {
            _autoMock = AutoMock.GetLoose();
        }

        [Fact]
        public async Task GetOrAddAsync_Test()
        {
            var moqIns = _autoMock.Create<RedisCacheProvider>(new TypedParameter(typeof(CacheOption), new CacheOption()
            {
                IpAddress = "192.168.190.4",
                Password = "qwe123QWE"
            }));
            var result = await moqIns.GetOrAddAsync("a", "123456789");

            Assert.Equal("123456789", result);
        }
    }
}
