using System.Collections.Concurrent;

namespace AtraBase.Caching;

/// <summary>
/// A simple thread-safe cache.
/// Deeply inspired by SMAPI's interval cache, just made thread-safe.
/// </summary>
/// <typeparam name="TKey">Type of key.</typeparam>
/// <typeparam name="TValue">Type of value.</typeparam>
public class SimpleConcurrentCache<TKey, TValue>
    where TKey : notnull
{
    private ConcurrentDictionary<TKey, TValue> cache = new();
    private ConcurrentDictionary<TKey, TValue> stale = new();

    /// <summary>The total number of items in the cache.</summary>
    /// <remarks>May slightly overcount when something is both in the hot cache and the stale cache.</remarks>
    public int Count => this.cache.Count + this.stale.Count;

    public bool IsReadOnly => false;

    public TValue GetOrSet(TKey key, Func<TValue> provider)
    {
        if (this.cache.TryGetValue(key, out TValue? value))
        {
            return value;
        }

        if (this.stale.TryGetValue(key, out value))
        {
            this.cache[key] = value;
            return value;
        }

        value = provider();
        this.cache[key] = value;
        return value;
    }

    /// <summary>
    /// Gets a value from the cache if possible. Writes to the hot cache.
    /// </summary>
    /// <param name="key">Cache key.</param>
    /// <returns>Value.</returns>
    /// <exception cref="KeyNotFoundException">Key not in collection.</exception>
    public TValue this[TKey key]
    {
        get
        {
            if (this.TryGetValue(key, out TValue? val))
            {
                return val;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        set
        {
            this.cache[key] = value;
        }
    }

    public void Swap()
    {
        this.stale.Clear();
        if (!this.cache.IsEmpty)
        {
            (this.stale, this.cache) = (this.cache, this.stale);
        }
    }

    /// <summary>
    /// Attempts to add the key and value to the hot cache.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    /// <returns>If it was successfully added.</returns>
    public bool TryAdd(TKey key, TValue value)
        => this.cache.TryAdd(key, value);

    public bool TryAdd(KeyValuePair<TKey, TValue> item)
        => this.cache.TryAdd(item.Key, item.Value);

    public void Clear()
    {
        this.cache.Clear();
        this.stale.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
        => this.cache.Contains(item) || this.stale.Contains(item);

    public bool ContainsKey(TKey key)
        => this.cache.ContainsKey(key) || this.stale.ContainsKey(key);

    public bool TryRemove(TKey key, out TValue? value)
    {
        bool success = this.cache.TryRemove(key, out value);
        if (success)
        {
            this.stale.TryRemove(key, out _);
            return true;
        }
        else
        {
            return this.stale.TryRemove(key, out value);
        }
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (this.cache.TryGetValue(key, out value))
        {
            return true;
        }
        return this.stale.TryGetValue(key, out value);
    }
}
