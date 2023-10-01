using CommunityToolkit.Diagnostics;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions similar to Linq
/// but using ref struct enumerators instead.
/// </summary>
public static class LinqLight
{
    public static FilterToType<TArray, TCast> FilterToType<TArray, TCast>(this TArray[] array)
        where TCast : TArray
    {
        Guard.IsNotNull(array);
        return new(array);
    }

    public static FilterToType<TArray, TCast> FilterToType<TArray, TCast>(this Span<TArray> span)
        where TCast : TArray
        => new(span);
}

/// <summary>
/// Iterates over a Span, returning only items that can be cast to a specific type.
/// </summary>
/// <typeparam name="TArray">Type of the span.</typeparam>
/// <typeparam name="TCast">Type to filter to.</typeparam>
public ref struct FilterToType<TArray, TCast>
    where TCast : TArray
{
    private readonly Span<TArray> span;
    private int index = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterToType{TArray, TCast}"/> struct.
    /// </summary>
    /// <param name="span">Span to iterate over.</param>
    public FilterToType(Span<TArray> span) => this.span = span;

    /// <inheritdoc cref="IEnumerator{T}.Current"/>
    public TCast? Current { get; private set; } = default;

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public FilterToType<TArray, TCast> GetEnumerator() => this;

    /// <inheritdoc cref="IEnumerator{T}.MoveNext"/>
    public bool MoveNext()
    {
        while (++this.index < this.span.Length)
        {
            if (this.span[this.index] is TCast cast)
            {
                this.Current = cast;
                return true;
            }
        }
        return false;
    }
}
