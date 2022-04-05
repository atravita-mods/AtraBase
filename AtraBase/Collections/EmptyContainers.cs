namespace AtraBase.Collections;

/// <summary>
/// Gets empty containers.
/// </summary>
internal static class EmptyContainers
{
    /// <summary>
    /// Gets a blank dictionary of the following type.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>An empty dictionary.</returns>
    internal static Dictionary<TKey, TValue> GetEmptyDictionary<TKey, TValue>()
        where TKey : notnull
        => new();

    /// <summary>
    /// Gets a blank list of the following type.
    /// </summary>
    /// <typeparam name="TKey">They key type.</typeparam>
    /// <returns>An empty list.</returns>
    internal static List<TKey> GetEmptyList<TKey>()
        => new();
}