namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Add some python-esque methods to the dictionaries.
/// </summary>
public static class DictionaryExtensions
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
        dictionary.TryAdd(key, defaultValue);
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
    public static TValue GetValueOrDefaultOverrideNull<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue defaultValue)
    {
        if (key is not null && dictionary.TryGetValue(key, out TValue? value) && value is not null)
        {
            return value;
        }
        else
        {
            return defaultValue;
        }
    }
}