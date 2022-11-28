namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions similar to Linq
/// but using ref struct enumerators instead.
/// </summary>
public static class LinqLight
{
    public static FilterToType<TArray, TCast> FilterToType<TArray, TCast>(this TArray[] array)
        where TCast : TArray
        => new(array);

    public static FilterToType<TArray, TCast> FilterToType<TArray, TCast>(this Span<TArray> span)
        where TCast : TArray
        => new(span);
}

public ref struct FilterToType<TArray, TCast>
    where TCast : TArray
{
    private readonly Span<TArray> span;
    private int index = -1;

    public FilterToType(Span<TArray> span) => this.span = span;

    /// <inheritdoc cref="IEnumerator{T}.Current"/>
    public TCast? Current { get; private set; } = default;

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public FilterToType<TArray, TCast> GetEnumerator() => this;

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
