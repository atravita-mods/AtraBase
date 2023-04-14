using System.Diagnostics;
using System.Runtime.CompilerServices;

using AtraBase.Models.Result;
using AtraBase.Toolkit.Extensions;

namespace AtraBase.Models.WeightedRandom;

[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Stylecop doesn't understand records.")]
public readonly record struct WeightedItem<T>(double Weight, T? Item);

/// <summary>
/// Wraps a list of weighted items in order to quickly produce values.
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
/// <remarks>This is geared towards reusing the same list of weights often.</remarks>
public class WeightedManager<T>
{
    private readonly List<WeightedItem<T?>> items = new();
    private double[]? processedChances;
    private double max = -1;

    private Random? random;

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedManager{T}"/> class.
    /// </summary>
    public WeightedManager()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedManager{T}"/> class.
    /// </summary>
    /// <param name="items">Initial items.</param>
    public WeightedManager(IEnumerable<WeightedItem<T?>> items)
        => this.items.AddRange(items);

    #endregion

    #region properties

    /// <summary>
    /// Gets the number of elements for this weighted list.
    /// </summary>
    public int Count => this.items.Count;

    /// <summary>
    /// Gets the random instance for this manager.
    /// Creates it and warms it up if necessary.
    /// </summary>
    private Random Random => this.random ??= new Random().PreWarm();

    #endregion

    #region mutators

    /// <summary>
    /// Sets a random for this weighted manager to be used with.
    /// </summary>
    /// <param name="random">The random.</param>
    /// <returns>the weighted manager.</returns>
    public WeightedManager<T?> WithRandom(Random? random)
    {
        this.random = random;
        return this;
    }

    /// <summary>
    /// Adds an element to the weighted manager.
    /// </summary>
    /// <param name="item">Weighted item.</param>
    public void Add(WeightedItem<T?> item)
    {
        if (item.Weight > 0)
        {
            this.Reset();
            this.items.Add(item);
        }
    }

    /// <summary>
    /// Adds an element to the weighted manager.
    /// </summary>
    /// <param name="weight">Weight to use.</param>
    /// <param name="item">Item to add.</param>
    public void Add(double weight, T? item)
    {
        if (weight > 0)
        {
            this.Reset();
            this.items.Add(new WeightedItem<T?>(weight, item));
        }
    }

    /// <summary>
    /// Adds a range of items to the weighted manager.
    /// </summary>
    /// <param name="items">IEnumerable of items.</param>
    public void AddRange(IEnumerable<WeightedItem<T?>> items)
    {
        this.Reset();
        this.items.AddRange(items.Where(item => item.Weight > 0));
    }

    /// <summary>
    /// Resets the precalculated array of chances.
    /// </summary>
    public void Reset()
        => this.processedChances = null;

    /// <summary>
    /// Clears both the items list and the processed chances.
    /// </summary>
    public void Clear()
    {
        this.items.Clear();
        this.Reset();
    }

    /// <summary>
    /// Removes a specific item.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    /// <returns>If an item was removed at all.</returns>
    public bool Remove(WeightedItem<T?> item)
    {
        this.Reset();
        return this.items.Remove(item);
    }

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">Index to remove at.</param>
    public void RemoveAt(int index)
    {
        this.Reset();
        this.items.RemoveAt(index);
    }

    #endregion

    /// <summary>
    /// Gets a single value from this weighted manager.
    /// </summary>
    /// <param name="random">The random to use.</param>
    /// <returns>value, or default if this WeightedManager is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public Option<T?> GetValue(Random? random = null)
    {
        if (this.items.Count == 0)
        {
            return default;
        }

        random ??= this.Random;

        if (this.processedChances is null || this.processedChances.Length != this.items.Count)
        {
            this.ProcessChances();
        }

        double chance = random.NextDouble() * this.max;

        int index = Array.BinarySearch(this.processedChances, chance);

        if (index < 0)
        {
            index = ~index - 1;
        }

        return new Option<T?>(this.items[index].Item);
    }

    /// <summary>
    /// Given a cutoff C and assuming M = sum(all weights), pick a weighted item M/C percent
    /// of the time and returns null otherwise (if M is less than C).
    ///
    /// Reverts to normal weighted random otherwise.
    /// </summary>
    /// <param name="cutoff">Cutoff to use.</param>
    /// <param name="random">Random instance.</param>
    /// <returns>Item, or default.</returns>
    public Option<T?> GetValue(double cutoff, Random? random = null)
    {
        if (this.items.Count == 0)
        {
            return default;
        }

        random ??= this.Random;

        if (this.processedChances is null || this.processedChances.Length != this.items.Count)
        {
            this.ProcessChances();
        }

        if (cutoff >= this.max || random.NextDouble() * cutoff < this.max)
        {
            return this.GetValue(random);
        }

        return default;
    }

    /// <summary>
    /// Gets an value without building the probability cache.
    /// </summary>
    /// <param name="random">Random to use.</param>
    /// <returns>Value, or default if this WeightedManager is empty.</returns>
    public Option<T?> GetValueUncached(Random? random = null)
    {
        if (this.items.Count == 0)
        {
            return default;
        }

        random ??= this.Random;

        // The values are cached already just use that.
        if (this.processedChances?.Length == this.items.Count)
        {
            return this.GetValue(random);
        }

        double acc = 0;

        foreach (WeightedItem<T?> item in this.items)
        {
            acc += item.Weight;
        }

        double chance = random.NextDouble() * acc;

        foreach (WeightedItem<T?> item in this.items)
        {
            chance -= item.Weight;
            if (chance <= 0 )
            {
                return new Option<T?>(item.Item);
            }
        }

        // it's really, really unlikely we'll get here but we might (float rounding)
        // so just fill in with the last item.
        Debug.Assert(false, "GetValueUncached hit emergency backup.");
        return new Option<T?>(this.items.Last().Item);
    }

    [MemberNotNull(nameof(processedChances))]
    private void ProcessChances()
    {
        this.processedChances = new double[this.items.Count];
        double acc = 0;

        for (int i = 0; i < this.items.Count; i++)
        {
            this.processedChances[i] = acc;
            acc += this.items[i].Weight;
        }

        this.max = acc;
    }
}