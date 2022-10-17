using System.Buffers;

namespace AtraBase.Models.WeightedRandom;

[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Stylecop doesn't understand records.")]
public readonly record struct WeightedItem<T>(double Weight, T Item);

/// <summary>
/// Wraps a list of weighted items in order to quickly produce values.
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
/// <remarks>This is geared towards reusing the same list of weights often.</remarks>
public class WeightedManager<T>
{
    private readonly List<WeightedItem<T>> items = new();
    private double[]? processedChances;
    private double max = -1;

    private Random? random;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedManager{T}"/> class.
    /// </summary>
    public WeightedManager() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedManager{T}"/> class.
    /// </summary>
    /// <param name="items">Initial items.</param>
    public WeightedManager(IEnumerable<WeightedItem<T>> items)
        => this.items.AddRange(items);

    /// <summary>
    /// Gets the random instance for this manager.
    /// Creates it and warms it up if necessary.
    /// </summary>
    private Random Random
    {
        get
        {
            if (this.random is null)
            {
                this.random = new();
                int warmup = this.random.Next(10, 30);
                for (int i = 0; i < warmup; i++)
                {
                    _ = this.random.NextDouble();
                }
            }
            return this.random;
        }
    }

    /// <summary>
    /// Gets the number of elements for this weighted list.
    /// </summary>
    public int Count => this.items.Count;

    public void Add(WeightedItem<T> item)
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
    public void Add(double weight, T item)
    {
        if (weight > 0)
        {
            this.Reset();
            this.items.Add(new WeightedItem<T>(weight, item));
        }
    }

    /// <summary>
    /// Adds a range of items to the weighted manager.
    /// </summary>
    /// <param name="items">IEnumerable of items.</param>
    public void AddRange(IEnumerable<WeightedItem<T>> items)
    {
        this.Reset();
        this.items.AddRange(items.Where(item => item.Weight > 0));
    }

    /// <summary>
    /// Resets pre-calculated array of chances.
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
    public bool Remove(WeightedItem<T> item)
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

    public T GetValue(Random? random = null)
    {
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

        return this.items[index].Item;
    }

    public T GetValueUncached(Random? random = null)
    {
        random ??= this.Random;

        // The values are cached already just use that.
        if (this.processedChances?.Length == this.items.Count)
        {
            return this.GetValue(random);
        }

        double acc = 0;
        double[] weights = ArrayPool<double>.Shared.Rent(this.items.Count);

        for (int i = 0; i < this.items.Count; i++)
        {
            weights[i] = acc;
            acc += this.items[i].Weight;
        }

        double chance = random.NextDouble() * acc;

        int index = Array.BinarySearch(weights, 0, this.items.Count, chance);

        if (index < 0)
        {
            index = ~index - 1;
        }

        // try not to leak memory.
        Array.Clear(weights, 0,  this.items.Count);
        ArrayPool<double>.Shared.Return(weights);

        return this.items[index].Item;
    }

    [MemberNotNull("processedChances")]
    private void ProcessChances()
    {
        this.processedChances = new double[this.items.Count];
        double acc = 0;

        for (int i = 0; i < this.items.Count; i++)
        {
            this.processedChances[i] = acc;
            WeightedItem<T> item = this.items[i];
            acc += item.Weight;
        }

        this.max = acc;
    }
}