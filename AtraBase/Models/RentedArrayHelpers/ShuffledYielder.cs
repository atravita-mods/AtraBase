using System.Runtime.CompilerServices;

namespace AtraBase.Models.RentedArrayHelpers;

/// <summary>
/// Extensions for ShuffledYielder.
/// </summary>
public static class ShuffleExtensions
{
    /// <summary>
    /// Modified Fisher-Yates shuffle that yields a single element at a time.
    /// </summary>
    /// <typeparam name="T">type param.</typeparam>
    /// <param name="span">Span to shuffle.</param>
    /// <param name="count">Count of the relevant items.</param>
    /// <param name="random">Random to use (null for a new one).</param>
    /// <returns>A ShuffleYielder ref enumerator.</returns>
    public static ShuffledYielder<T> Shuffled<T>(this Span<T> span, int? count, Random? random)
        where T : struct
        => new(span, count, random);
}

/// <summary>
/// Modified Fisher-Yates shuffle that yields out one item at a time.
/// </summary>
/// <typeparam name="T">The type of thing to yield.</typeparam>
/// <remarks>Built for use with rented arrays.</remarks>
public ref struct ShuffledYielder<T>
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

    /// <inheritdoc cref="IEnumerator{T}.Current"/>
    public T? Current { get; private set; } = default;

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public readonly ShuffledYielder<T> GetEnumerator() => this;

    /// <summary>
    /// Moves to the next value in the collection.
    /// </summary>
    /// <returns>True if there are more values, false otherwise.</returns>
    [MemberNotNullWhen(true, nameof(Current))]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        if (this.index < 0)
        {
            return false;
        }

        int j = this.random.Next(this.index + 1);
        this.Current = this.span[j]!;
        if (j != this.index)
        {
            this.span[j] = this.span[this.index];
            this.span[this.index] = this.Current;
        }

        this.index--;
        return true;
    }
}
