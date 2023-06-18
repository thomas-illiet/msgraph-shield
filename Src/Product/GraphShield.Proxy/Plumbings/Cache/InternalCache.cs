using Microsoft.Extensions.Caching.Memory;

namespace GraphShield.Proxy.Plumbings.Cache
{
    /// <summary>
    /// Represents a wrapper that re-exposes existing methods and adds a missing method by replacing the MemoryCache object with a new one.
    /// </summary>
    internal class InternalCache : IInternalCache
    {
        private readonly object _serviceLock = new();
        private IMemoryCache _memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalCache"/> class.
        /// </summary>
        public InternalCache()
        {
            Reset();
        }

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The located value or null.</param>
        /// <returns>True if the key was found.</returns>
        public bool TryGetValue<TItem>(object key, out TItem? value)
        {
            lock (_serviceLock)
            {
                if (_memoryCache.TryGetValue(key, out object? result) && result is TItem item)
                {
                    value = item;
                    return true;
                }

                value = default;
                return false;
            }
        }

        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <returns>The newly created <see cref="ICacheEntry"/> instance.</returns>
        public ICacheEntry CreateEntry(object key)
        {
            lock (_serviceLock)
            {
                return _memoryCache.CreateEntry(key);
            }
        }

        /// <summary>
        /// Sets the specified key with the given value.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set<TItem>(object key, TItem value)
        {
            lock (_serviceLock)
            {
                _memoryCache.Set(key, value);
            }
        }

        /// <summary>
        /// Sets the specified key with the given value and absolute expiration.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="absoluteExpiration">The absolute expiration time.</param>
        public void Set<TItem>(object key, TItem value, DateTimeOffset absoluteExpiration)
        {
            lock (_serviceLock)
            {
                _memoryCache.Set(key, value, absoluteExpiration);
            }
        }

        /// <summary>
        /// Sets the specified key with the given value and relative expiration.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="absoluteExpirationRelativeToNow">The relative expiration time from now.</param>
        public void Set<TItem>(object key, TItem value, TimeSpan absoluteExpirationRelativeToNow)
        {
            lock (_serviceLock)
            {
                _memoryCache.Set(key, value, absoluteExpirationRelativeToNow);
            }
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void Remove(object key)
        {
            lock (_serviceLock)
            {
                _memoryCache.Remove(key);
            }
        }

        /// <summary>
        /// Resets the memory cache instance.
        /// </summary>
        public void Reset()
        {
            lock (_serviceLock)
            {
                _memoryCache?.Dispose();
                _memoryCache = new MemoryCache(new MemoryCacheOptions());
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (_serviceLock)
            {
                _memoryCache?.Dispose();
            }
        }
    }
}
