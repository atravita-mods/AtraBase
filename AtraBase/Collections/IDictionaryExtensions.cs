using CommunityToolkit.Diagnostics;

namespace AtraBase.Collections;

public static class IDictionaryExtensions
{
    public static bool IsEquivalentTo<TKey, TValue>(this IDictionary<TKey, TValue> self, IDictionary<TKey, TValue> other)
    {
        Guard.IsNotNull(self);
        Guard.IsNotNull(other);
        return self.Count == other.Count
            && self.All((kvp) => other.TryGetValue(kvp.Key, out TValue? val) && val?.Equals(kvp.Value) == true);
    }
}

