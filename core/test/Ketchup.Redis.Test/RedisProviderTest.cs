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
        private readonly RedisCacheProvider redisCache;


        public RedisProviderTest()
        {
            _autoMock = AutoMock.GetLoose();

            redisCache = _autoMock.Create<RedisCacheProvider>(new TypedParameter(typeof(CacheOption), new CacheOption()
            {
                IpAddress = "192.168.180.55",
                Password = "qwe123QWE"
            }));
        }

        [Fact]
        public async Task GetOrAddAsync_Test()
        {
            var result = await redisCache.GetOrAddAsync("a", "123456789");

            Assert.Equal("123456789", result);
        }

        [Fact]
        public async Task GetOrAddAsync_TimeSpane_Test()
        {
            var result = await redisCache.GetOrAddAsync("b", "123456789", TimeSpan.FromSeconds(10));

            Assert.Equal("123456789", result);
        }

        [Fact]
        public async Task GetAsync_Test()
        {
            var one = await redisCache.GetAsync<string>("a");

            Assert.Equal("123456789", one);
        }

        [Fact]
        public async Task AddAsync_Test()
        {
            await redisCache.AddAsync<string>("b", "test");

            var one = await redisCache.GetAsync<string>("b");

            Assert.Equal("test", one);
        }

        [Fact]
        public async Task Remove_Test()
        {
            await redisCache.RemoveAsync("a");

            var one = await redisCache.GetAsync<string>("a");

            Assert.Null(one);
        }
    }
}
