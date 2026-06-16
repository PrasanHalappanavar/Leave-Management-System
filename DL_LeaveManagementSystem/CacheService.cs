using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_LeaveManagementSystem
{
    public class CacheService
    {
        private static CacheService _instance;
        private static readonly object _lock = new object();

        private readonly IMemoryCache _cache;

        private CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public static CacheService GetCacheInstance(IMemoryCache cache = null)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CacheService(cache);
                    }
                }
            }
            return _instance;
        }


        public T Get<T>(string key)
        {
            _cache.TryGetValue(key, out T value);
            return value;
        }

        public void Set<T>(string key, T value, int minutesToExpire = 30)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutesToExpire)
            };
            _cache.Set(key, value, options);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }
    }
}
