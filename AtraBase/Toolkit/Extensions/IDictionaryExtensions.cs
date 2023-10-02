using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Add some python-esque methods to the dictionaries.
/// </summary>
public static class IDictionaryExtensions
{
    /// <summary>
    /// equivalent to python's dictionary.update().
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary">Dictionary to update.</param>
    /// <param name="updateDict">Dictionary containing values to add to the first dictionary.</param>
    /// <returns>the dictionary (for chaining).</returns>
    public static IDictionary<TKey, TValue> Update<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue>? updateDict)
    {
        if (updateDict is not null)
        {
            foreach (TKey key in updateDict.Keys)
            {
                dictionary[key] = updateDict[key];
            }
        }
        return dictionary;
    }

    /// <summary>
    /// equivalent to python's dictionary.update().
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary">Dictionary to update.</param>
    /// <param name="keyValuePairs">Array of key value pairs to add.</param>
    /// <returns>the dictionary (for chaining).</returns>
    public static IDictionary<TKey, TValue> Update<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        KeyValuePair<TKey, TValue>[]? keyValuePairs)
    {
        if (keyValuePairs is not null)
        {
            foreach ((TKey k, TValue v) in keyValuePairs)
            {
                dictionary[k] = v;
            }
        }
        return dictionary;
    }

    /// <summary>
    /// equivalent to python's dictionary.setdefault().
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary">Dictionary.</param>
    /// <param name="key">Key to look for.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <returns>Value from dictionary if one exists, else default value.</returns>
    /// <remarks>Function both sets state and returns value.</remarks>
    public static TValue? SetDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue defaultValue)
    {
        // add the value to the dictionary if it doesn't exist.
        if (dictionary.TryAdd(key, defaultValue))
        {
            return defaultValue;
        }
        return dictionary[key];
    }

    /// <summary>
    /// similar to SetDefault, but will override a null value.
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary">Dictionary to search in.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultValue">Value to use.</param>
    /// <returns>Value from dictionary if it exists and is not null, defaultValue otherwise.</returns>
    public static TValue SetDefaultOverrideNull<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue defaultValue)
    {
        if (dictionary.TryGetValue(key, out TValue? value) && value is not null)
        {
            return value;
        }
        else
        {
            dictionary[key] = defaultValue;
            return defaultValue;
        }
    }

    /// <summary>
    /// Retrieves a value from the dictionary.
    /// Uses the default if the value is null, or if the key is not found.
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary">Dictionary to search in.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <returns>Value from dictionary if not null, or else defaultValue.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue GetValueOrDefaultOverrideNull<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue defaultValue)
        => key is not null && dictionary.TryGetValue(key, out TValue? value) && value is not null ? value : defaultValue;

    /// <summary>
    /// Tries to retrieve a value from a dictionary, returning default if it cannot be found.
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary">Dictionary to search in.</param>
    /// <param name="key">Key.</param>
    /// <returns>Value if found, else default.</returns>
    /// <remarks>If value types are involved, consider the <see cref="CollectionsMarshal"/> methods instead.</remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue? GetValueOrGetDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key)
            where TValue : class
        => key is not null && dictionary.TryGetValue(key, out TValue? value) ? value : default;
}