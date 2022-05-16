using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FptEcommerce.API.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            var value = _cache.GetString(key);

            if (value != null)
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public T Set<T>(string key, T value, int m1, int m2)
        {
            var timeOut = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(m1),
                SlidingExpiration = TimeSpan.FromMinutes(m2)
            };

            _cache.SetString(key, JsonSerializer.Serialize(value), timeOut);

            return value;
        }
    }
}
