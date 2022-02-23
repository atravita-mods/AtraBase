using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// LINQ-like extensions on enumerables.
/// </summary>
internal static class IEnumerableExtensions
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
        [NotNull] this IEnumerable<TEnumerable> enumerable,
        [NotNull] Func<TEnumerable, TKey> keyselector,
        [NotNull] Func<TEnumerable, TValue> valueselector)
        where TEnumerable : notnull
        where TKey : notnull
        where TValue : notnull
    {
        Dictionary<TKey, TValue> result = new();
        foreach (TEnumerable item in enumerable)
        {
            if (!result.TryAdd(keyselector(item), valueselector(item)))
            {
                Console.WriteLine($"Recieved duplicate key {keyselector(item)}, ignoring");
            }
        }
        return result;
    }
}
