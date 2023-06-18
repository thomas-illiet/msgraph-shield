using System;
using Microsoft.Extensions.Caching.Memory;

namespace GraphShield.Proxy.Plumbings.Cache
{
    /// <summary>
    /// Represents an internal cache interface.
    /// </summary>
    public interface IInternalCache : IDisposable
    {
        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The located value or null.</param>
        /// <returns>True if the key was found.</returns>
        bool TryGetValue<TItem>(object key, out TItem? value);

        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <returns>The newly created <see cref="ICacheEntry"/> instance.</returns>
        ICacheEntry CreateEntry(object key);

        /// <summary>
        /// Sets the specified key with the given value.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Set<TItem>(object key, TItem value);

        /// <summary>
        /// Sets the specified key with the given value and absolute expiration.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="absoluteExpiration">The absolute expiration time.</param>
        void Set<TItem>(object key, TItem value, DateTimeOffset absoluteExpiration);

        /// <summary>
        /// Sets the specified key with the given value and relative expiration.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="absoluteExpirationRelativeToNow">The relative expiration time from now.</param>
        void Set<TItem>(object key, TItem value, TimeSpan absoluteExpirationRelativeToNow);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        void Remove(object key);

        /// <summary>
        /// Resets the memory cache instance.
        /// </summary>
        void Reset();
    }
}
