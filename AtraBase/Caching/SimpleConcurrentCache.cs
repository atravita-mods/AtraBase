using System.Collections.Concurrent;

namespace AtraBase.Caching;

/// <summary>
/// A simple thread-safe cache.
/// Deeply inspired by SMAPI's interval cache, just made thread-safe.
/// </summary>
/// <typeparam name="TKey">Type of key.</typeparam>
/// <typeparam name="TValue">Type of value.</typeparam>
public class SimpleConcurrentCache<TKey, TValue> : IDisposable
    where TKey : notnull
{
    private readonly Timer timer;

    private ConcurrentDictionary<TKey, TValue> cache;
    private ConcurrentDictionary<TKey, TValue> stale;

    /// <summary>
    /// Gets the timer for this instance.
    /// </summary>
    internal Timer Timer => this.timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with default timing.
    /// </summary>
    public SimpleConcurrentCache()
    {
        this.timer = new(
            new TimerCallback(this.OnTimerCallback),
            null,
            TimeSpan.FromMinutes(2),
            TimeSpan.FromMinutes(5));
        this.cache = new();
        this.stale = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="time">The amount of time needed between resets.</param>
    public SimpleConcurrentCache(TimeSpan time)
    {
        this.timer = new(
            new TimerCallback(this.OnTimerCallback),
            null,
            time,
            time);
        this.cache = new();
        this.stale = new();
    }

    /// <summary>Gets the total number of items in the cache.</summary>
    /// <remarks>May slightly overcount when something is both in the hot cache and the stale cache.</remarks>
    public int Count => this.cache.Count + this.stale.Count;

    /// <summary>
    /// Gets a value indicating whether or not the cache is read only.
    /// </summary>
    public bool IsReadOnly => false;

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

    /// <summary>
    /// Gets a value from the cache, or sets a value given by the provider.
    /// </summary>
    /// <param name="key">key.</param>
    /// <param name="provider">Function that provides a value.</param>
    /// <returns>The value from the cache or provider.</returns>
    public TValue GetOrSet(TKey key, Func<TValue> provider)
    {
        if (this.TryGetValue(key, out TValue? value))
        {
            return value;
        }

        value = provider();
        this.cache[key] = value;
        return value;
    }

    /// <summary>
    /// Swaps the two caches and clears the state cache.
    /// </summary>
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

    /// <summary>
    /// Attempts to add a value to the hot cache.
    /// </summary>
    /// <param name="item">kvp pair.</param>
    /// <returns>If successfully added.</returns>
    public bool TryAdd(KeyValuePair<TKey, TValue> item)
        => this.cache.TryAdd(item.Key, item.Value);

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public void Clear()
    {
        this.cache.Clear();
        this.stale.Clear();
    }

    /// <summary>
    /// If either cache contains the item.
    /// </summary>
    /// <param name="item">Item to search for.</param>
    /// <returns>If the kvp can be found in either cache.</returns>
    /// <remarks>Likely O(n).</remarks>
    public bool Contains(KeyValuePair<TKey, TValue> item)
        => this.cache.Contains(item) || this.stale.Contains(item);

    /// <summary>
    /// If either cache contains that key.
    /// </summary>
    /// <param name="key">Key to search for.</param>
    /// <returns>True if key either cache has the key.</returns>
    public bool ContainsKey(TKey key)
        => this.cache.ContainsKey(key) || this.stale.ContainsKey(key);

    /// <summary>
    /// Attempts to remove and return a value from either cache.
    /// If found in the hot cache, will remove from the state as well.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">returned value.</param>
    /// <returns>True if found and removed.</returns>
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

    /// <summary>
    /// Tries to get a value from this cache.
    /// </summary>
    /// <param name="key">Key to look for.</param>
    /// <param name="value">Out param, returned value.</param>
    /// <returns>True if successful, false otherwise.</returns>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (this.cache.TryGetValue(key, out value))
        {
            return true;
        }
        if (this.stale.TryGetValue(key, out value))
        {
            // promote to hot.
            this.cache[key] = value;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Called when this is disposed.
    /// This is primarily used to dispose of the internal timer.
    /// </summary>
    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "Dipose is used here to remove the inner timer. This class still will need to be finalized.")]
    public void Dispose()
    {
        this.timer.Change(TimeSpan.MaxValue, TimeSpan.MaxValue);
        this.timer.Dispose();
    }

    private void OnTimerCallback(object? state)
    {
        try
        {
            this.Swap();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AtraBase] Cache swap failed\n\n{ex}");
        }
    }
}
