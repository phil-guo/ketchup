using System;
using System.Threading.Tasks;
using CSRedis;
using Ketchup.Caching.Configurations;

namespace Ketchup.Caching.Internal.Redis
{
    public class RedisCacheProvider : ICacheProvider
    {

        public RedisCacheProvider(CacheOption cacheOption)
        {
            var redisClient = new CSRedisClient(
                 $"{cacheOption.IpAddress},password={cacheOption.Password},defaultDatabase={cacheOption.DefaultDatabase},poolsize={cacheOption.PoolSize},ssl=false,writeBuffer={cacheOption.WriteBuffer},prefix={cacheOption.Prefix}");

            RedisHelper.Initialization(redisClient);
        }

        public async Task<T> GetOrAddAsync<T>(string key, T value)
        {
            return await GetOrAddAsync<T>(key, value, null);
        }

        public async Task<T> GetOrAddAsync<T>(string key, T value, TimeSpan? slidingExpiration = null)
        {
            var data = await RedisHelper.GetAsync<T>(key);
            if (data != null)
                return value;
            if (slidingExpiration.HasValue)
                await RedisHelper.SetAsync(key, value, (TimeSpan)slidingExpiration);
            else
                await RedisHelper.SetAsync(key, value);

            return value;
        }

        public async Task RemoveAsync(string key)
        {
            await RedisHelper.DelAsync(key);
        }
    }
}