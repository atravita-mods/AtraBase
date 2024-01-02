using System.Collections.Concurrent;
using AtraBase.Internal;
using CommunityToolkit.Diagnostics;

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
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with default timing and a default comparer.
    /// </summary>
    public SimpleConcurrentCache()
    {
        this.timer = this.GetDefaultTimer();
        this.cache = new ();
        this.stale = new ();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with a default comparer.
    /// </summary>
    /// <param name="time">The amount of time needed between resets.</param>
    public SimpleConcurrentCache(TimeSpan time)
    {
        this.timer = new (
            new TimerCallback(this.OnTimerCallback),
            null,
            time,
            time);
        this.cache = new ();
        this.stale = new ();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with default timing and a default comparer.
    /// </summary>
    /// <param name="initial">Initial elements to populate the hot cache with.</param>
    public SimpleConcurrentCache(IEnumerable<KeyValuePair<TKey, TValue>> initial)
    {
        Guard.IsNotNull(initial);

        this.timer = this.GetDefaultTimer();
        this.cache = new (initial);
        this.stale = new ();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with the default comparer.
    /// </summary>
    /// <param name="initial">Initial elements to populate the hot cache with.</param>
    /// <param name="time">The amount of time between resets.</param>
    public SimpleConcurrentCache(IEnumerable<KeyValuePair<TKey, TValue>> initial, TimeSpan time)
    {
        Guard.IsNotNull(initial);

        this.timer = new (
            new TimerCallback(this.OnTimerCallback),
            null,
            time,
            time);
        this.cache = new (initial);
        this.stale = new ();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with a custom comparer and default timing.
    /// </summary>
    /// <param name="comparer">Custom comparer.</param>
    public SimpleConcurrentCache(IEqualityComparer<TKey> comparer)
    {
        Guard.IsNotNull(comparer);

        this.timer = this.GetDefaultTimer();
        this.cache = new (comparer);
        this.stale = new (comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class using a custom comparer and time period.
    /// </summary>
    /// <param name="comparer">Comparer.</param>
    /// <param name="time">Time period to use.</param>
    public SimpleConcurrentCache(IEqualityComparer<TKey> comparer, TimeSpan time)
    {
        Guard.IsNotNull(comparer);

        this.timer = new (
            new TimerCallback(this.OnTimerCallback),
            null,
            time,
            time);

        this.cache = new (comparer);
        this.stale = new (comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with default timing and a custom comparer.
    /// </summary>
    /// <param name="initial">Initial collection to populate the hot cache.</param>
    /// <param name="comparer">Custom comparer.</param>
    public SimpleConcurrentCache(IEnumerable<KeyValuePair<TKey, TValue>> initial, IEqualityComparer<TKey> comparer)
    {
        Guard.IsNotNull(initial);
        Guard.IsNotNull(comparer);

        this.timer = this.GetDefaultTimer();
        this.cache = new (initial, comparer);
        this.stale = new (comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with custom timing and a custom comparer.
    /// </summary>
    /// <param name="initial">Initial collection to populate the hot cache.</param>
    /// <param name="comparer">Custom comparer.</param>
    /// <param name="time">Time period to use.</param>
    public SimpleConcurrentCache(IEnumerable<KeyValuePair<TKey, TValue>> initial, IEqualityComparer<TKey> comparer, TimeSpan time)
    {
        Guard.IsNotNull(initial);
        Guard.IsNotNull(comparer);

        this.timer = new (
            new (this.OnTimerCallback),
            null,
            time,
            time);

        this.cache = new (initial, comparer);
        this.stale = new (comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with the specified concurrency level.
    /// </summary>
    /// <param name="concurrencyLevel">Estimated concurrency level.</param>
    /// <param name="capacity">Initial capacity.</param>
    /// <remarks>Doesn't adjust the concurrency level for the timer.</remarks>
    public SimpleConcurrentCache(int concurrencyLevel, int capacity)
    {
        Guard.IsGreaterThanOrEqualTo(concurrencyLevel, 1);
        Guard.IsGreaterThanOrEqualTo(capacity, 0);

        this.timer = this.GetDefaultTimer();
        this.cache = new (concurrencyLevel, capacity);
        this.stale = new (concurrencyLevel, 0); // no need to reserve capacity for the stale cache.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with the given estimated concurrency level, capacity, and timeframe.
    /// </summary>
    /// <param name="concurrencyLevel">Estimated concurrency level.</param>
    /// <param name="capacity">Initial capacity.</param>
    /// <param name="time">Time between resets.</param>
    /// <remarks>Does not adjust the concurrency level for the timer.</remarks>
    public SimpleConcurrentCache(int concurrencyLevel, int capacity, TimeSpan time)
    {
        Guard.IsGreaterThanOrEqualTo(concurrencyLevel, 1);
        Guard.IsGreaterThanOrEqualTo(capacity, 0);

        this.timer = new (
            new TimerCallback(this.OnTimerCallback),
            null,
            time,
            time);
        this.cache = new (concurrencyLevel, capacity);
        this.stale = new (concurrencyLevel, 0); // no need to reserve capacity for the stale cache.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with the given initial elements, a custom comparer,
    /// and the given concurrency level, but with default timings.
    /// </summary>
    /// <param name="concurrencyLevel">Estimated concurrency level.</param>
    /// <param name="initial">Initial collection to populate the hot cache.</param>
    /// <param name="comparer">Comparer to use.</param>
    /// <remarks>Does not adjust the concurrency level for the timer.</remarks>
    public SimpleConcurrentCache(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> initial, IEqualityComparer<TKey> comparer)
    {
        Guard.IsGreaterThanOrEqualTo(concurrencyLevel, 1);
        Guard.IsNotNull(initial);
        Guard.IsNotNull(comparer);

        this.timer = this.GetDefaultTimer();
        this.cache = new (concurrencyLevel, initial, comparer);
        this.stale = new (concurrencyLevel, Enumerable.Empty<KeyValuePair<TKey, TValue>>(), comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with the given initial elements, a custom comparer,
    /// and the given concurrency level, and custom timing.
    /// </summary>
    /// <param name="concurrencyLevel">Estimated concurrency level.</param>
    /// <param name="initial">Initial collection to populate the hot cache.</param>
    /// <param name="comparer">Comparer to use.</param>
    /// <param name="time">The time between resets.</param>
    /// <remarks>Does not adjust the concurrency level for the timer.</remarks>
    public SimpleConcurrentCache(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> initial, IEqualityComparer<TKey> comparer, TimeSpan time)
    {
        Guard.IsGreaterThanOrEqualTo(concurrencyLevel, 1);
        Guard.IsNotNull(initial);
        Guard.IsNotNull(comparer);

        this.timer = new (
            new TimerCallback(this.OnTimerCallback),
            null,
            time,
            time);
        this.cache = new (concurrencyLevel, initial, comparer);
        this.stale = new (concurrencyLevel, Enumerable.Empty<KeyValuePair<TKey, TValue>>(), comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with the given concurrency level and capacity, and a custom comparer,
    /// but default timings.
    /// </summary>
    /// <param name="concurrencyLevel">Estimated concurrency level.</param>
    /// <param name="capacity">Expected capacity.</param>
    /// <param name="comparer">Custom comparer to use.</param>
    public SimpleConcurrentCache(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
    {
        Guard.IsGreaterThanOrEqualTo(concurrencyLevel, 1);
        Guard.IsGreaterThanOrEqualTo(capacity, 0);
        Guard.IsNotNull(comparer);

        this.timer = this.GetDefaultTimer();
        this.cache = new (concurrencyLevel, capacity, comparer);
        this.stale = new (concurrencyLevel, 0, comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleConcurrentCache{TKey, TValue}"/> class with the given concurrency level and capacity, a custom comparer,
    /// and the given time between resets.
    /// </summary>
    /// <param name="concurrencyLevel">Estimated concurrency level.</param>
    /// <param name="capacity">Expected capacity.</param>
    /// <param name="comparer">Custom comparer to use.</param>
    /// <param name="time">The expected time.</param>
    public SimpleConcurrentCache(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer, TimeSpan time)
    {
        Guard.IsGreaterThanOrEqualTo(concurrencyLevel, 1);
        Guard.IsGreaterThanOrEqualTo(capacity, 0);
        Guard.IsNotNull(comparer);

        this.timer = new (
            new TimerCallback(this.OnTimerCallback),
            null,
            time,
            time);
        this.cache = new (concurrencyLevel, capacity, comparer);
        this.stale = new (concurrencyLevel, 0, comparer);
    }

    /// <summary>Gets the total number of items in the cache.</summary>
    /// <remarks>May slightly overcount when something is both in the hot cache and the stale cache.</remarks>
    public int Count => this.cache.Count + this.stale.Count;

    /// <summary>
    /// Gets a value indicating whether or not the cache is read only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets the timer for this instance.
    /// </summary>
    internal Timer Timer => this.timer;

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
        ConcurrentDictionary<TKey, TValue> cache = this.cache;
        ConcurrentDictionary<TKey, TValue> stale = this.stale;
#if DEBUG
        Logger.Instance.Info($"{this.cache.Count} in hot cache, {this.stale.Count} in the stale cache before swap.");
#endif
        stale.Clear();
        if (!cache.IsEmpty)
        {
            this.stale = cache;
            this.cache = stale;
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
    /// <param name="item">Key Value Pair.</param>
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
    /// <returns>If the KVP can be found in either cache.</returns>
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
        ConcurrentDictionary<TKey, TValue> cache = this.cache;
        ConcurrentDictionary<TKey, TValue> stale = this.stale;

        if (cache.TryRemove(key, out value))
        {
            stale.TryRemove(key, out _);
            return true;
        }
        else
        {
            return stale.TryRemove(key, out value);
        }
    }

    /// <summary>
    /// Tries to get a value from this cache.
    /// </summary>
    /// <param name="key">Key to look for.</param>
    /// <param name="value">Out parameter, returned value.</param>
    /// <returns>True if successful, false otherwise.</returns>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        ConcurrentDictionary<TKey, TValue> cache = this.cache;
        ConcurrentDictionary<TKey, TValue> stale = this.stale;

        if (cache.TryGetValue(key, out value))
        {
            return true;
        }
        if (stale.TryGetValue(key, out value))
        {
            // promote to hot.
            cache[key] = value;
            return true;
        }
        return false;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Called when this is disposed.
    /// This is primarily used to dispose of the internal timer.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        this.timer.Change(TimeSpan.MaxValue, TimeSpan.MaxValue);
        this.timer.Dispose();
        this.cache = null!;
        this.stale = null!;
    }

    private void OnTimerCallback(object? state)
    {
        try
        {
            this.Swap();
        }
        catch (Exception ex)
        {
            Logger.Instance.Error($"[AtraBase] Cache swap failed.", ex);
        }
    }

    /// <summary>
    /// Gets the default timer.
    /// This one does the first reset after two minutes, and does five minutes between others.
    /// This is because a LOT of reflection happens at startup, and less so later on.
    /// </summary>
    /// <returns>The default timer.</returns>
    private Timer GetDefaultTimer()
    => new (
        new TimerCallback(this.OnTimerCallback),
        null,
        TimeSpan.FromMinutes(2),
        TimeSpan.FromMinutes(5));
}
