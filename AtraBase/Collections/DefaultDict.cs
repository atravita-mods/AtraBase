#if COLLECTIONS

using System.Collections;

namespace AtraBase.Collections;

/// <summary>
/// Similar to Python's defaultdict.
/// </summary>
/// <typeparam name="TKey">Type of key.</typeparam>
/// <typeparam name="TValue">Type of value.</typeparam>
internal class DefaultDict<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : new()
{
    private readonly Dictionary<TKey, TValue> dict;

    private readonly Func<TValue> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDict{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="factory">The function for which to generate new values. If left null, just creates a new TValue().</param>
    internal DefaultDict(Func<TValue>? factory = null)
    {
        this.factory = factory ?? (() => new TValue());
        this.dict = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDict{TKey, TValue}"/> class. Copies values from dictionary.
    /// </summary>
    /// <param name="dictionary">Dictionary to copy from.</param>
    /// <param name="factory">The function for which to generate new values. If left null, just creates a new TValue().</param>
    internal DefaultDict(IDictionary<TKey, TValue> dictionary, Func<TValue>? factory = null)
    {
        this.factory = factory ?? (() => new TValue());
        this.dict = new(dictionary);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDict{TKey, TValue}"/> class. Copies values from the IEumerable.
    /// </summary>
    /// <param name="collection">IEnumerable to get initial values from.</param>
    /// <param name="factory">The function for which to generate new values. If left null, just creates a new TValue().</param>
    internal DefaultDict(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TValue>? factory = null)
    {
        this.factory = factory ?? (() => new TValue());
        this.dict = new(collection);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDict{TKey, TValue}"/> class with a custom comparer.
    /// </summary>
    /// <param name="comparer">Custom comparer.</param>
    /// <param name="factory">The function for which to generate new values. If left null, just creates a new TValue().</param>
    internal DefaultDict(IEqualityComparer<TKey> comparer, Func<TValue>? factory = null)
    {
        this.factory = factory ?? (() => new TValue());
        this.dict = new(comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDict{TKey, TValue}"/> class with an initial capacity.
    /// </summary>
    /// <param name="capacity">Initial capacity.</param>
    /// <param name="factory">The function for which to generate new values. If left null, just creates a new TValue().</param>
    internal DefaultDict(int capacity, Func<TValue>? factory = null)
    {
        this.factory = factory ?? (() => new TValue());
        this.dict = new(capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDict{TKey, TValue}"/> class, with initial values from dictionary and a custom comparer.
    /// </summary>
    /// <param name="dictionary">Dictionary to copy from.</param>
    /// <param name="comparer">Custom comparer.</param>
    /// <param name="factory">The function for which to generate new values. If left null, just creates a new TValue().</param>
    internal DefaultDict(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer, Func<TValue>? factory = null)
    {
        this.factory = factory ?? (() => new TValue());
        this.dict = new(dictionary, comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDict{TKey, TValue}"/> class. Copies values from the IEumerable and uses a custom comparer.
    /// </summary>
    /// <param name="collection">IEnumerable to get initial values from.</param>
    /// <param name="comparer">Custom comparer.</param>
    /// <param name="factory">The function for which to generate new values. If left null, just creates a new TValue().</param>
    internal DefaultDict(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer, Func<TValue>? factory = null)
    {
        this.factory = factory ?? (() => new TValue());
        this.dict = new(collection, comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDict{TKey, TValue}"/> class with an initial capacity and a custom comparer.
    /// </summary>
    /// <param name="capacity">Initial capacity.</param>
    /// <param name="comparer">Custom comparer.</param>
    /// <param name="factory">The function for which to generate new values. If left null, just creates a new TValue().</param>
    internal DefaultDict(int capacity, IEqualityComparer<TKey> comparer, Func<TValue>? factory = null)
    {
        this.factory = factory ?? (() => new TValue());
        this.dict = new(capacity, comparer);
    }

    /// <summary>Gets the value of the dictionary if it exists. If not, uses the factory to create a new value.</summary>
    /// <param name="key">Key to search for.</param>
    public TValue this[TKey key]
    {
        get
        {
            if (this.dict.TryGetValue(key, out TValue? value))
            {
                return value;
            }
            else
            {
                TValue val = this.factory();
                this.dict[key] = val;
                return val;
            }
        }

        set
        {
            this.dict[key] = value;
        }
    }

    /// <inheritdoc/>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Reviewed")]
    public ICollection<TKey> Keys => this.dict.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => this.dict.Values;

    /// <inheritdoc/>
    public int Count => this.dict.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)this.dict).IsReadOnly;

    /// <inheritdoc/>
    public void Add(TKey key, TValue value) => this.dict.Add(key, value);

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        this.dict.Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public void Clear() => this.dict.Clear();

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item) => this.dict.Contains(item);

    /// <inheritdoc/>
    public bool ContainsKey(TKey key) => this.dict.ContainsKey(key);

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        for (int i = arrayIndex; i < array.Length; i++)
        {
            (TKey k, TValue v) = array[i];
            this.dict[k] = v;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.dict.GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(TKey key) => this.dict.Remove(key);

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this.dict).Remove(item);

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => this.dict.TryGetValue(key, out value);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.dict.GetEnumerator();
}

#endif