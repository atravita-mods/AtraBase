#if COLLECTIONS

using System.Collections;
using System.Text;
using AtraBase.Toolkit.Extensions;

namespace AtraBase.Collections;

/// <summary>
/// Similar to Python's Counter.
/// </summary>
/// <typeparam name="TKey">Type of key.</typeparam>
internal class Counter<TKey> : IDictionary<TKey, int>
    where TKey : notnull
{
    private readonly Dictionary<TKey, int> dict;

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter{TKey}"/> class.
    /// </summary>
    internal Counter() => this.dict = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter{TKey}"/> class, with values copied from a different counter.
    /// </summary>
    /// <param name="other">Other counter to copy from.</param>
    internal Counter(Counter<TKey> other)
    {
        this.dict = new();
        this.dict.Update(other.dict);
    }

    /// <summary>Gets the value of the dictionary if it exists. If not, returns 0.</summary>
    /// <param name="key">Key to search for.</param>
    public int this[TKey key]
    {
        get
        {
            if (this.dict.TryGetValue(key, out int val))
            {
                return val;
            }
            else
            {
                this.dict[key] = 0;
                return 0;
            }
        }

        set
        {
            this.dict[key] = value;
        }
    }

    /// <summary>
    /// Updates one counter with the values from the other.
    /// </summary>
    /// <param name="other">The other counter.</param>
    /// <remarks>Adds counts, not replaces.</remarks>
    internal void Update(Counter<TKey> other)
    {
        foreach ((TKey k, int v) in other)
        {
            this[k] = this[k] + v;
        }
    }

    internal void Update(IEnumerable<TKey> enumerable)
    {
        foreach (TKey key in enumerable)
        {
            this[key] = this[key] + 1;
        }
    }

    internal void Subtract(Counter<TKey> other)
    {
        foreach ((TKey k, int v) in other)
        {
            this[k] = this[k] - v;
        }
    }

    internal void Subtract(IEnumerable<TKey> enumerable)
    {
        foreach (TKey key in enumerable)
        {
            this[key] = this[key] - 1;
        }
    }

    /// <summary>
    /// Remove all zero values.
    /// </summary>
    internal void RemoveZeros()
    {
        List<TKey> toRemove = new();
        foreach ((TKey key, int count) in this)
        {
            if (count == 0)
            {
                toRemove.Add(key);
            }
        }
        foreach (TKey key in toRemove)
        {
            this.Remove(key);
        }
    }

    /// <summary>
    /// Removes all counts if they're below a certain limit. (Inclusive).
    /// </summary>
    /// <param name="limit">Limit to remove all values under.</param>
    internal void RemoveBelow(int limit = 0)
    {
        List<TKey> toRemove = new();
        foreach ((TKey key, int count) in this)
        {
            if (count <= limit)
            {
                toRemove.Add(key);
            }
        }
        foreach (TKey key in toRemove)
        {
            this.Remove(key);
        }
    }

#pragma warning disable SA1201 // Elements should appear in the correct order - methods unique to Counter are placed above methods common to dictionaries.
#pragma warning disable SA1202 // Elements should be ordered by access

    /// <summary>
    /// Not implemented - does not make sense for Counter.
    /// </summary>
    /// <param name="array">not relevant, not implemented.</param>
    /// <param name="arrayIndex">not relevant, so not implemented.</param>
    /// <exception cref="NotSupportedException">Not implemented! Does not make sense for Counter.</exception>
    public void CopyTo(KeyValuePair<TKey, int>[] array, int arrayIndex)
        => throw new NotSupportedException("This method makes no sense for Counter.");

    /// <inheritdoc/>
    public ICollection<TKey> Keys => this.dict.Keys;

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append("Counter");
        foreach ((TKey key, int count) in this.dict)
        {
            sb.AppendLine().Append(key.ToString()).Append(": ").Append(count);
        }
        return sb.ToString();
    }

    /// <inheritdoc/>
    public ICollection<int> Values => this.dict.Values;

    /// <inheritdoc/>
    public int Count => this.dict.Count;

    /// <summary>
    /// Gets the sum of the counts.
    /// </summary>
    public int Total => this.dict.Values.Sum();

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, int>>)this.dict).IsReadOnly;

    /// <inheritdoc/>
    public void Add(TKey key, int value) => this.dict.Add(key, value);

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, int> item) => this.dict.Add(item.Key, item.Value);

    /// <inheritdoc/>
    public void Clear() => this.dict.Clear();

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, int> item) => this.dict.Contains(item);

    /// <inheritdoc/>
    public bool ContainsKey(TKey key) => this.dict.ContainsKey(key);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, int>> GetEnumerator() => this.dict.GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(TKey key) => this.dict.Remove(key);

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, int> item) => ((ICollection<KeyValuePair<TKey, int>>)this.dict).Remove(item);

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out int value) => this.dict.TryGetValue(key, out value);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.dict.GetEnumerator();

#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1202 // Elements should be ordered by access
}

#endif