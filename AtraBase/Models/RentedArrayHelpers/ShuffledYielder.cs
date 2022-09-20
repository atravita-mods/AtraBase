namespace AtraBase.Models.RentedArrayHelpers;

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
    private int index;
    private Random random;

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
