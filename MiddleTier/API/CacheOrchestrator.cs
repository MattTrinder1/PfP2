using Microsoft.Extensions.Caching.Memory;
using System;

namespace API
{
    /// <summary>
    /// A wrapper around IMemoryCache to prevent never expiring cache entries.
    /// </summary>
    public class CacheOrchestrator
    {
        public const int DefaultExpiryMinutes = 720;
        private IMemoryCache cache;

        public CacheOrchestrator(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public void Remove(object key)
        {
            cache.Remove(key);
        }

        public TItem Set<TItem>(object key, TItem value)
        {
            return cache.Set(key, value, TimeSpan.FromMinutes(DefaultExpiryMinutes));
        }

        public TItem Set<TItem>(object key, TItem value, TimeSpan absoluteExpirationRelativeToNow)
        {
            return cache.Set(key, value, absoluteExpirationRelativeToNow);
        }

        public TItem Set<TItem>(object key, TItem value, DateTimeOffset absoluteExpiration)
        {
            return cache.Set(key, value, absoluteExpiration);
        }

        public bool TryGetValue<TItem>(object key, out TItem value)
        {
            return cache.TryGetValue(key, out value);
        }
    }
}
