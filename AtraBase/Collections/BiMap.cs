using System.Collections;
using CommunityToolkit.Diagnostics;

namespace AtraBase.Collections;

/********************
 * Consulted https://gitlab.com/daleao/sdv-mods/-/blob/75d6c6f4366af28fcc627d15b345f68d8dc18b74/ImmersiveValley/Common/Classes/Instance/BidirectionalMap.cs
 * while writing this.
 * It's pretty different though.
 *******************/

/// <summary>
/// A simple bi-directional dictionary.
/// Not thread safe.
/// </summary>
/// <remarks>Often uses .Add to enforce the bimap, be ready for throws.</remarks>
/// <typeparam name="TForward">Type of keys for the forward map.</typeparam>
/// <typeparam name="TReverse">Type of keys for the reverse map.</typeparam>
public class BiMap<TForward, TReverse> : IEnumerable<KeyValuePair<TForward, TReverse>>
    where TForward : notnull
    where TReverse : notnull
{
    private readonly Dictionary<TForward, TReverse> forward;
    private readonly Dictionary<TReverse, TForward> reverse;

    /// <summary>
    /// Initializes a new instance of the <see cref="BiMap{TForward, TReverse}"/> class that is empty and uses the default comparer.
    /// </summary>
    public BiMap()
    {
        this.forward = new Dictionary<TForward, TReverse>();
        this.reverse = new Dictionary<TReverse, TForward>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiMap{TForward, TReverse}"/> class that contains the values from a specfic dictionary.
    /// </summary>
    /// <param name="dictionary">Dictionary to copy from.</param>
    /// <exception cref="ArgumentException">Duplicate value.</exception>
    /// <exception cref="ArgumentNullException">Value was null.</exception>
    public BiMap(Dictionary<TForward, TReverse> dictionary)
    {
        Guard.IsNotNull(dictionary);

        // process reverse first
        // it'll throw if there's a duplicate or the value is null
        // value will never be null if nullable is enforced but this is public.
        this.reverse = new(dictionary.Count);
        foreach ((TForward k, TReverse v) in dictionary)
        {
            this.reverse.Add(v, k);
        }
        this.forward = new(dictionary);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiMap{TForward, TReverse}"/> class that contains the values from a specfic collection.
    /// Uses the default comparer.
    /// </summary>
    /// <param name="collection">Dictionary to copy from.</param>
    /// <exception cref="ArgumentException">Duplicate value.</exception>
    /// <exception cref="ArgumentNullException">Value was null.</exception>
    public BiMap(IEnumerable<KeyValuePair<TForward, TReverse>> collection)
    {
        Guard.IsNotNull(collection);

        // construct the forward collection.
        this.forward = new(collection);

        // construct the reverse collection.
        this.reverse = new(this.forward.Count);
        foreach ((TForward k, TReverse v) in this.forward)
        {
            this.reverse.Add(v, k);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiMap{TForward, TReverse}"/> class that is empty.
    /// Uses custom comparers.
    /// </summary>
    /// <param name="forwardComparer">custom comparer for the forward map. Leave null to be default.</param>
    /// <param name="reverseComparer">custom comparer for the reverse map, leave null to be default.</param>
    public BiMap(IEqualityComparer<TForward>? forwardComparer, IEqualityComparer<TReverse>? reverseComparer)
    {
        this.forward = forwardComparer is null ? new() : new(forwardComparer);
        this.reverse = reverseComparer is null ? new() : new(reverseComparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiMap{TForward, TReverse}"/> class with the given capacity and default comparers.
    /// </summary>
    /// <param name="capacity">starting capacity.</param>
    public BiMap(int capacity)
    {
        Guard.IsGreaterThanOrEqualTo(capacity, 0);
        this.forward = new(capacity);
        this.reverse = new(capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiMap{TForward, TReverse}"/> class from the dictionary.
    /// Uses custom comparers.
    /// </summary>
    /// <param name="dictionary">Dictionary to copy from.</param>
    /// <param name="forwardComparer">custom comparer for the forward map. Leave null to be default.</param>
    /// <param name="reverseComparer">custom comparer for the reverse map, leave null to be default.</param>
    /// <exception cref="ArgumentException">Duplicate value.</exception>
    /// <exception cref="ArgumentNullException">Value was null.</exception>
    public BiMap(Dictionary<TForward, TReverse> dictionary, IEqualityComparer<TForward>? forwardComparer, IEqualityComparer<TReverse>? reverseComparer)
    {
        Guard.IsNotNull(dictionary);

        // process reverse first
        // it'll throw if there's a duplicate or the value is null
        // value will never be null if nullable is enforced but this is public.
        this.reverse = reverseComparer is null ? new(dictionary.Count) : new(dictionary.Count, reverseComparer);
        foreach ((TForward k, TReverse v) in dictionary)
        {
            this.reverse.Add(v, k);
        }

        this.forward = forwardComparer is null ? new(dictionary) : new(dictionary, forwardComparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiMap{TForward, TReverse}"/> class that contains the values from a specfic collection.
    /// Uses custom comparers.
    /// </summary>
    /// <param name="collection">Dictionary to copy from.</param>
    /// <param name="forwardComparer">custom comparer for the forward map. Leave null to be default.</param>
    /// <param name="reverseComparer">custom comparer for the reverse map, leave null to be default.</param>
    /// <exception cref="ArgumentException">Duplicate value.</exception>
    /// <exception cref="ArgumentNullException">Value was null.</exception>
    public BiMap(IEnumerable<KeyValuePair<TForward, TReverse>> collection, IEqualityComparer<TForward>? forwardComparer, IEqualityComparer<TReverse>? reverseComparer)
    {
        Guard.IsNotNull(collection);

        // process forward map
        this.forward = forwardComparer is null ? new(collection) : new(collection, forwardComparer);

        // process reverse map. Allow Add to throw if there will be a problem
        this.reverse = reverseComparer is null ? new(this.forward.Count) : new(this.forward.Count, reverseComparer);
        foreach ((TForward k, TReverse v) in this.forward)
        {
            this.reverse.Add(v, k);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiMap{TForward, TReverse}"/> class with the given capacity and custom comparers.
    /// </summary>
    /// <param name="capacity">starting capacity.</param>
    /// <param name="forwardComparer">custom comparer for the forward map. Leave null to be default.</param>
    /// <param name="reverseComparer">custom comparer for the reverse map, leave null to be default.</param>
    public BiMap(int capacity, IEqualityComparer<TForward>? forwardComparer, IEqualityComparer<TReverse>? reverseComparer)
    {
        Guard.IsGreaterThanOrEqualTo(capacity, 0);
        this.forward = forwardComparer is null ? new(capacity) : new(capacity, forwardComparer);
        this.reverse = reverseComparer is null ? new(capacity) : new(capacity, reverseComparer);
    }

    /**************
     * BiMap properties
     * ************/

    /// <summary>
    /// Gets a collection that contains the forward keys in this bimap.
    /// </summary>
    public ICollection<TForward> ForwardKeys => this.forward.Keys;

    /// <summary>
    /// Gets a collection that contains the reverse keys in this bimap.
    /// </summary>
    public ICollection<TReverse> ReverseKeys => this.reverse.Keys;

    /// <summary>
    /// Gets a count of the items in this bimap.
    /// </summary>
    public int Count => this.forward.Count;

    /// <summary>
    /// Gets a value indicating whether or not this collection is readonly. (It's not.)
    /// </summary>
    public bool IsReadOnly => false;

    /*************
     * BiMap methods
     * Do not implement an indexer, that doesn't even make sense lol.
     * Be careful to not let people fuck up the data structure's constraints.
     * **********/

    /// <summary>
    /// Adds the specified set of values to the bimap.
    /// </summary>
    /// <param name="forward">Forward value.</param>
    /// <param name="reverse">Reverse value.</param>
    /// <exception cref="ArgumentException">Duplicate (in either direction).</exception>
    /// <exception cref="ArgumentNullException">Forward or reverse value was null.</exception>
    public void Add(TForward forward, TReverse reverse)
    {
        // guard ain't liking the fact that I'm trying to use it with a type with a notnull constraint.
        // this is just for safety.
        if (forward is null)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(forward));
        }
        if (reverse is null)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(reverse));
        }

        // check first - avoid mutating either dictionary if the operation cannot be completed.
        if (this.forward.ContainsKey(forward) || this.reverse.ContainsKey(reverse))
        {
            ThrowHelper.ThrowArgumentException($"Either {forward} or {reverse} already exists in the collection!");
        }
        this.forward.Add(forward, reverse);
        this.reverse.Add(reverse, forward);
    }

    /// <summary>
    /// Adds the specified item to the bimap.
    /// </summary>
    /// <param name="item">KVP corresponding to the entry to add.</param>
    /// <exception cref="ArgumentException">Duplicate (in either direction).</exception>
    /// <exception cref="ArgumentNullException">Forward or reverse value was null.</exception>
    public void Add(KeyValuePair<TForward, TReverse> item) => this.Add(item.Key, item.Value);

    /// <summary>
    /// Tries to add a set of values to the bimap.
    /// </summary>
    /// <param name="forward">forward value.</param>
    /// <param name="reverse">reverse value.</param>
    /// <returns>true if successfully added, false otherwise.</returns>
    public bool TryAdd(TForward forward, TReverse reverse)
    {
        if (forward is null || reverse is null)
        {
            return false;
        }

        if (this.forward.ContainsKey(forward) || this.reverse.ContainsKey(reverse))
        {
            return false;
        }

        this.forward.Add(forward, reverse);
        this.reverse.Add(reverse, forward);
        return true;
    }

    /// <summary>
    /// Tries to add an item to the bimap.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <returns>true if successfully added, false otherwise.</returns>
    public bool TryAdd(KeyValuePair<TForward, TReverse> item) => this.TryAdd(item.Key, item.Value);

    /// <summary>
    /// Clears the bimap.
    /// </summary>
    public void Clear()
    {
        this.forward.Clear();
        this.reverse.Clear();
    }

    /// <summary>
    /// Whether or not the bimap contains this pair.
    /// </summary>
    /// <param name="forward">forward value.</param>
    /// <param name="reverse">reverse value.</param>
    /// <returns>true if the pair is contained in the bimap. If either doesn't match, will return false.</returns>
    /// <exception cref="ArgumentNullException">Forward or reverse value was null.</exception>
    public bool Contains(TForward forward, TReverse reverse)
    {
        if (forward is null || reverse is null)
        {
            return ThrowHelper.ThrowArgumentNullException<bool>("Either forward or reverse were null");
        }
        return this.forward.ContainsKey(forward) && this.reverse.ContainsKey(reverse);
    }

    /// <summary>
    /// Whether or not the bimap contains this item.
    /// </summary>
    /// <param name="item">KVP corresponding to the pair.</param>
    /// <returns>true if the pair is contained in the bimap. If either doesn't match, will return false.</returns>
    /// <exception cref="ArgumentNullException">Forward or reverse value was null.</exception>
    public bool Contains(KeyValuePair<TForward, TReverse> item) => this.Contains(item.Key, item.Value);

    /// <summary>
    /// If the forward map contains the key in question.
    /// </summary>
    /// <param name="forward">key.</param>
    /// <returns>True if the forward map contains the key.</returns>
    public bool ContainsForwardKey(TForward forward) => this.forward.ContainsKey(forward);

    /// <summary>
    /// If the reverse map contains the key in question.
    /// </summary>
    /// <param name="reverse">key.</param>
    /// <returns>True if the reverse map contains the key.</returns>
    public bool ContainsReverseKey(TReverse reverse) => this.reverse.ContainsKey(reverse);

    /// <summary>
    /// Tries to get a value out of the forward map.
    /// </summary>
    /// <param name="forward">Forward key.</param>
    /// <param name="reverse">Out param, the value.</param>
    /// <returns>if the value was successfully gotten.</returns>
    public bool TryGetForward(TForward forward, [MaybeNullWhen(false)] out TReverse reverse) => this.forward.TryGetValue(forward, out reverse);

    /// <summary>
    /// Tries to get a value out of the reverse map.
    /// </summary>
    /// <param name="reverse">Reverse key.</param>
    /// <param name="forward">Out param, the value.</param>
    /// <returns>If the value was successfully gotten.</returns>
    public bool TryGetReverse(TReverse reverse, [MaybeNullWhen(false)] out TForward forward) => this.reverse.TryGetValue(reverse, out forward);

#warning should probably test this logic?

    /// <summary>
    /// Tries to remove a pair from the bimap.
    /// </summary>
    /// <param name="forward">Forward value.</param>
    /// <param name="reverse">Reverse value.</param>
    /// <returns>true if removed.</returns>
    public bool Remove(TForward forward, TReverse reverse)
    {
        if (forward is null || reverse is null)
        {
            return ThrowHelper.ThrowArgumentNullException<bool>("Either forward or reverse were null");
        }
        if (!this.forward.TryGetValue(forward, out TReverse? rev) || this.reverse.Comparer.Equals(rev, reverse)
            || !this.reverse.TryGetValue(reverse, out TForward? fore) || this.forward.Comparer.Equals(fore, forward))
        {
            return false;
        }
        this.forward.Remove(forward);
        this.reverse.Remove(reverse);
        return true;
    }

    /// <summary>
    /// Tries to remove an item from the bimap.
    /// </summary>
    /// <param name="item">item to remove.</param>
    /// <returns>true if removed.</returns>
    public bool Remove(KeyValuePair<TForward, TReverse> item) => this.Remove(item.Key, item.Value);

    /// <summary>
    /// Removes the item in the bimap that corresponds to the
    /// forward key.
    /// </summary>
    /// <param name="forward">key.</param>
    /// <returns>True if successfully removed, false otherwise.</returns>
    public bool RemoveKeyFromForward(TForward forward)
    {
        if (forward is null)
        {
            return ThrowHelper.ThrowArgumentException<bool>("forward was null");
        }
        if (this.forward.TryGetValue(forward, out TReverse? reverse))
        {
            this.forward.Remove(forward);
            this.reverse.Remove(reverse);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes the item in the bimap that corresponds to the
    /// reverse key.
    /// </summary>
    /// <param name="reverse">key.</param>
    /// <returns>True if successfully removed, false otherwise.</returns>
    public bool RemoveKeyFromReverse(TReverse reverse)
    {
        if (reverse is null)
        {
            return ThrowHelper.ThrowArgumentException<bool>("reverse was null");
        }
        if (this.reverse.TryGetValue(reverse, out TForward? forward))
        {
            this.forward.Remove(forward);
            this.reverse.Remove(reverse);
            return true;
        }
        return false;
    }

    /************
     * Interface methods
     * **********/

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TForward, TReverse>> GetEnumerator() => this.forward.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.forward.GetEnumerator();
}
