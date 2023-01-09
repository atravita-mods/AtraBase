using CommunityToolkit.Diagnostics;

namespace AtraBase.Collections;

/// <summary>
/// Extensions for IDictionary.
/// </summary>
public static class IDictionaryExtensions
{
    /// <summary>
    /// Whether two dictionaries are equivalent.
    ///
    /// Two dictionaries are considered equivalent if they have the same number of elements
    /// and the same key will get equivalent values out of each dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="self"></param>
    /// <param name="other"></param>
    /// <returns>True if equivalent, false otherwise.</returns>
    public static bool IsEquivalentTo<TKey, TValue>(this IDictionary<TKey, TValue> self, IDictionary<TKey, TValue> other)
        where TValue : IEquatable<TValue>
    {
        Guard.IsNotNull(self);
        Guard.IsNotNull(other);
        return self.Count == other.Count
            && self.All((kvp) => other.TryGetValue(kvp.Key, out TValue? val) && val?.Equals(kvp.Value) == true);
    }
}