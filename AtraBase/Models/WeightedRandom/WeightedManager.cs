namespace AtraBase.Models.WeightedRandom;

public record struct WeightedItem<T>(double Weight, T Item);

/// <summary>
/// Wraps a list of weighted items in order to quickly produce values.
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
/// <remarks>This is geared towards reusing the same list of weights often.</remarks>
public class WeightedManager<T>
{
    private List<WeightedItem<T>> items = new();
    private double[]? processedChances;
    private double max = -1;

    private Random? random;

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

    public WeightedManager() { }

    public WeightedManager(IEnumerable<WeightedItem<T>> items)
        => this.items.AddRange(items);

    public void Add(WeightedItem<T> item)
    {
        this.Reset();
        this.items.Add(item);
    }

    public void AddRange(IEnumerable<WeightedItem<T>> items)
    {
        this.Reset();
        this.items.AddRange(items);
    }

    public void Reset()
        => this.processedChances = null;

    public bool Remove(WeightedItem<T> item)
    {
        this.Reset();
        return this.items.Remove(item);
    }

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