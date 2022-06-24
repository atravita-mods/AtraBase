namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// LINQ-like extensions on enumerables.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Similar to LINQ's ToDictionary, but ignores duplicates instead of erroring.
    /// </summary>
    /// <typeparam name="TEnumerable">The type of elements in the enumerable.</typeparam>
    /// <typeparam name="TKey">The type of the keys in the resulting dictionary.</typeparam>
    /// <typeparam name="TValue">They type of the values in the resulting dictionary.</typeparam>
    /// <param name="enumerable">The enumerable to look at.</param>
    /// <param name="keyselector">Function that maps enumerable to key.</param>
    /// <param name="valueselector">Function that maps enumerable to value.</param>
    /// <returns>The dictionary.</returns>
    [Pure]
    public static Dictionary<TKey, TValue> ToDictionaryIgnoreDuplicates<TEnumerable, TKey, TValue>(
        this IEnumerable<TEnumerable> enumerable,
        Func<TEnumerable, TKey> keyselector,
        Func<TEnumerable, TValue> valueselector)
        where TKey : notnull
    {
        Dictionary<TKey, TValue> result = new();
        foreach (TEnumerable item in enumerable)
        {
            if (!result.TryAdd(keyselector(item), valueselector(item)))
            {
#if DEBUG
                Console.WriteLine($"Recieved duplicate key {keyselector(item)}, ignoring");
#endif
            }
        }
        return result;
    }

    /// <summary>
    /// Uses a hashset to check if items are unique.
    /// </summary>
    /// <typeparam name="T">Type of enumerable.</typeparam>
    /// <param name="enumerable">Enumerable to check.</param>
    /// <returns>An enumerable of unique items.</returns>
    /// <remarks>This implementation is probably pretty slow, eh.</remarks>
    [Pure]
    public static IEnumerable<T> Unique<T>(this IEnumerable<T> enumerable)
    {
        HashSet<T> hashed = enumerable.ToHashSet();
        foreach (T item in hashed)
        {
            yield return item;
        }
    }
}
