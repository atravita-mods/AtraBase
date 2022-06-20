namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Helper methods to deal with spans.
/// </summary>
internal static class SpanExtensions
{
    internal static Span<char> Concat(this Span<char> thisspan, Span<char> thatspan)
    {
        char[] buffer = GC.AllocateUninitializedArray<char>(thisspan.Length + thatspan.Length);
        var span = new Span<char>(buffer);
        thisspan.CopyTo(span);
        thatspan.CopyTo(span[thisspan.Length..]);
        return span;
    }

    internal static Span<char> Concat(this Span<char> thisspan, Span<char> thatspan1, Span<char> thatspan2)
    {
        char[] buffer = GC.AllocateUninitializedArray<char>(thisspan.Length + thatspan1.Length + thatspan2.Length);
        var span = new Span<char>(buffer);
        int index = 0;
        thisspan.CopyTo(span);
        index += thisspan.Length;
        thatspan1.CopyTo(span[index..]);
        index += thatspan1.Length;
        thatspan2.CopyTo(span[index..]);
        return span;
    }
}
