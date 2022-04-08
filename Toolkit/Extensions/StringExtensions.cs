namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extension methods on strings.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Gets the Nth occurance of a specific unicode char in a string.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <param name="item">Char to search for.</param>
    /// <param name="count">N.</param>
    /// <returns>Index of the char, or -1 if not found.</returns>
    [Pure]
    internal static int NthOccuranceOf(this string str, char item, int count = 1)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == item && --count <= 0)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Gets the Nth occurance from the end of a specific unicode char in a string.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <param name="item">Char to search for.</param>
    /// <param name="count">N.</param>
    /// <returns>Index of the char, or -1 if not found.</returns>
    [Pure]
    internal static int NthOccuranceFromEnd(this string str, char item, int count = 1)
    {
        for (int i = str.Length - 1; i >= 0; i--)
        {
            if (str[i] == item && --count <= 0)
            {
                return i;
            }
        }
        return -1;
    }
}