using System;
using System.Threading.Tasks;

namespace Ketchup.Caching.Internal
{
    public interface ICacheProvider
    {
        Task<T> GetAsync<T>(string key);
        Task AddAsync<T>(string key, T value);
        Task AddAsync<T>(string key, T value, TimeSpan? expiration);
        Task<T> GetOrAddAsync<T>(string key, T value);
        Task<T> GetOrAddAsync<T>(string key, T value, TimeSpan? slidingExpiration = null);
        Task RemoveAsync(string key);
    }
}
