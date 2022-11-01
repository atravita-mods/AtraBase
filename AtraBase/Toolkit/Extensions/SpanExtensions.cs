using System.Runtime.CompilerServices;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Helper methods to deal with spans.
/// </summary>
public static class SpanExtensions
{
    /// <summary>
    /// Concatenates two spans.
    /// </summary>
    /// <param name="thisspan">First span.</param>
    /// <param name="thatspan">Second span.</param>
    /// <returns>Combined span.</returns>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static Span<char> Concat(this Span<char> thisspan, Span<char> thatspan)
    {
        char[] buffer = GC.AllocateUninitializedArray<char>(thisspan.Length + thatspan.Length);
        Span<char> span = new(buffer);
        thisspan.CopyTo(span);
        thatspan.CopyTo(span[thisspan.Length..]);
        return span;
    }

    /// <summary>
    /// Concatenates three spans.
    /// </summary>
    /// <param name="thisspan">First span.</param>
    /// <param name="thatspan1">Second span.</param>
    /// <param name="thatspan2">Third span.</param>
    /// <returns>Combined span.</returns>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static Span<char> Concat(this Span<char> thisspan, Span<char> thatspan1, Span<char> thatspan2)
    {
        char[] buffer = GC.AllocateUninitializedArray<char>(thisspan.Length + thatspan1.Length + thatspan2.Length);
        Span<char> span = new(buffer);
        int index = 0;
        thisspan.CopyTo(span);
        index += thisspan.Length;
        thatspan1.CopyTo(span[index..]);
        index += thatspan1.Length;
        thatspan2.CopyTo(span[index..]);
        return span;
    }
}
