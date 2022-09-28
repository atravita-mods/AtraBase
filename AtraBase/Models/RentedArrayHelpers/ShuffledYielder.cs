namespace AtraBase.Models.RentedArrayHelpers;

/// <summary>
/// Extensions for ShuffledYielder.
/// </summary>
public static class ShuffleExtensions
{
    public static ShuffledYielder<T> Shuffled<T>(this Span<T> span, int? count, Random? random)
        where T: struct
        => new(span, count, random);
}

public ref struct ShuffledYielder<T>
    where T : struct
{
    private readonly Span<T> span;
    private readonly Random random;
    private int index;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShuffledYielder{T}"/> struct.
    /// </summary>
    /// <param name="span">Span to yield from.</param>
    /// <param name="count">Effective length of the Span. Leave null to use the full span.</param>
    /// <param name="random">Random to use. Null will use a new instance.</param>
    public ShuffledYielder(Span<T> span, int? count = null, Random? random = null)
    {
        this.span = span;
        this.index = (count ?? span.Length) - 1;
        this.random = random ?? new();
    }

    public ShuffledYielder<T> GetEnumerator() => this;

    public T Current { get; private set; } = default;

    public bool MoveNext()
    {
        if (this.index < 0)
        {
            return false;
        }

        int j = this.random.Next(this.index);
        this.Current = this.span[j];
        this.span[j] = this.span[this.index];
        this.span[this.index] = this.Current;

        this.index--;
        return true;
    }
}
