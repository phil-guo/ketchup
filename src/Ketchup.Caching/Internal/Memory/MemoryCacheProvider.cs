using System;
using System.Threading.Tasks;
using Ketchup.Core.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Ketchup.Caching.Internal.Memory
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly MemoryCache Default;

        public MemoryCacheProvider()
        {
            Default = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await Task.Run(() => Default.Get<T>(key));
        }

        public async Task AddAsync<T>(string key, T value)
        {
            await AddAsync(key, value, null);
        }

        public async Task AddAsync<T>(string key, T value, TimeSpan? expiration)
        {
            await Task.Run(() => Default.Set(key, value, (TimeSpan)expiration));
        }

        public async Task<T> GetOrAddAsync<T>(string key, T value)
        {
            return await GetOrAddAsync<T>(key, value, TimeSpan.FromSeconds(30));
        }

        public async Task<T> GetOrAddAsync<T>(string key, T value, TimeSpan? slidingExpiration = null)
        {
            return await Default.GetOrCreateAsync<T>(key, factory =>
                {
                    if (slidingExpiration.HasValue)
                        factory.SetSlidingExpiration((TimeSpan)slidingExpiration);

                    return Task.FromResult(value);
                });
        }

        public async Task RemoveAsync(string key)
        {
            await Task.Run(() => Default.Remove(key));
        }
    }
}

