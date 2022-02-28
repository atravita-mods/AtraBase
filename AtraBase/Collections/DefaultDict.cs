using System.Collections;

namespace AtraBase.Collections;

/// <summary>
/// Similar to Python's defaultdict. TODO: figure out custom comparers?
/// </summary>
/// <typeparam name="TKey">Type of key.</typeparam>
/// <typeparam name="TValue">Type of value.</typeparam>
internal class DefaultDict<TKey, TValue> : IDictionary<TKey, TValue>
    where TValue : new()
{
    private readonly Dictionary<TKey, TValue> dict = new();

    private readonly Func<TValue> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDict{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="factory">The function for which to generate new values. If left null, just creates a new TValue().</param>
    public DefaultDict(Func<TValue>? factory = null)
    {
        this.factory = factory ?? (() => new TValue());
    }

    /// <inheritdoc/>
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