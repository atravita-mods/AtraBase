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
    /// <returns>Dictionary with the name of the matchgroup as the key and the value as the value.</returns>
    [Pure]
    public static Dictionary<string, string> MatchGroupsToDictionary([NotNull] this Match match)
    {
        Dictionary<string, string> result = new();
        foreach (Group group in match.Groups)
        {
            result[group.Name] = group.Value;
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
    /// <returns>The dictionary.</returns>
    [Pure]
    public static Dictionary<TKey, TValue> MatchGroupsToDictionary<TKey, TValue>(
        [NotNull] this Match match,
        [NotNull] Func<string, TKey> keyselector,
        [NotNull] Func<string, TValue> valueselector)
        where TKey : notnull
        where TValue : notnull
    {
        Dictionary<TKey, TValue> result = new();
        foreach (Group group in match.Groups)
        {
            result[keyselector(group.Name)] = valueselector(group.Value);
        }
        return result;
    }
}