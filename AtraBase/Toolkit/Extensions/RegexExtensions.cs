using System.Text.RegularExpressions;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Adds some LINQ-esque methods to the Regex class.
/// </summary>
public static class RegexExtensions
{
    /// <summary>
    /// Converts a Match with named matchgroups into a dictionary.
    /// </summary>
    /// <param name="match">Regex matchgroup.</param>
    /// <param name="namedOnly">Whether or not to only use named groups.</param>
    /// <returns>Dictionary with the name of the matchgroup as the key and the value as the value.</returns>
    [Pure]
    public static Dictionary<string, string> MatchGroupsToDictionary(this Match match, bool namedOnly = true)
    {
        Dictionary<string, string> result = [];
        foreach (Group group in match.Groups)
        {
            // Microsoft doesn't really give a better way to exclude unnamed groups, which are numbers as strings.
            if (!namedOnly || !int.TryParse(group.Name, out _))
            {
                result[group.Name] = group.Value;
            }
        }
        return result;
    }

    /// <summary>
    /// Converts a Match with named matchgroups into a dictionary.
    /// </summary>
    /// <typeparam name="TKey">Type for key.</typeparam>
    /// <typeparam name="TValue">Type for value.</typeparam>
    /// <param name="match">Match with named matchgroups.</param>
    /// <param name="keyselector">Function to apply to all keys.</param>
    /// <param name="valueselector">Function to apply to all values.</param>
    /// <param name="namedOnly">Whether or not to only use named groups.</param>
    /// <returns>The dictionary.</returns>
    [Pure]
    public static Dictionary<TKey, TValue> MatchGroupsToDictionary<TKey, TValue>(
        this Match match,
        Func<string, TKey> keyselector,
        Func<string, TValue> valueselector,
        bool namedOnly = true)
        where TKey : notnull
    {
        Dictionary<TKey, TValue> result = [];
        foreach (Group group in match.Groups)
        {
            // Microsoft doesn't really give a better way to exclude unnamed groups, which are numbers as strings.
            if (!namedOnly || !int.TryParse(group.Name, out _))
            {
                result[keyselector(group.Name)] = valueselector(group.Value);
            }
        }
        return result;
    }

    /// <summary>
    /// Updates a dictionary with the values of a match group, if valid.
    /// </summary>
    /// <param name="dictionary">Dictionary to update.</param>
    /// <param name="match">Match with named matchgroups.</param>
    /// <param name="namedOnly">Whether or not to only use named groups.</param>
    /// <returns>The dictionary (for chaining).</returns>
    public static IDictionary<string, int> Update(
        this IDictionary<string, int> dictionary,
        Match? match,
        bool namedOnly = true)
    {
        if (match is not null)
        {
            foreach (Group group in match.Groups)
            {
                // Microsoft doesn't really give a better way to exclude unnamed groups, which are numbers as strings.
                if (!namedOnly || !int.TryParse(group.Name, out _))
                {
                    if (int.TryParse(group.Value, out int val))
                    {
                        dictionary[group.Name] = val;
                    }
                }
            }
        }
        return dictionary;
    }
}