namespace AtraBase.Toolkit.Shims.NetSix;

#if !NET6_0_OR_GREATER

/// <summary>
/// Finds the minimum and maximum of an IEnumerable with a custom comparer.
/// </summary>
internal static class IEnumerableShims
{
    public static T? Min<T>(this IEnumerable<T> items, IComparer<T> comparer)
    {
        T? first = items.FirstOrDefault(x => x is not null);
        if (first is null)
        {
            return default;
        }
        foreach (T? item in items)
        {
            if (comparer.Compare(item, first) < 0)
            {
                first = item;
            }
        }
        return first;
    }

    public static T? Max<T>(this IEnumerable<T> items, IComparer<T> comparer)
    {
        T? first = items.FirstOrDefault(x => x is not null);
        if (first is null)
        {
            return default;
        }
        foreach (T? item in items)
        {
            if (comparer.Compare(item, first) > 0)
            {
                first = item;
            }
        }
        return first;
    }
}

#endif
