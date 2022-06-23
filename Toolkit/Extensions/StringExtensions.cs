using Microsoft.Toolkit.Diagnostics;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extension methods on strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Gets the Nth occurance of a specific unicode char in a string.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <param name="item">Char to search for.</param>
    /// <param name="count">N.</param>
    /// <returns>Index of the char, or -1 if not found.</returns>
    [Pure]
    public static int NthOccuranceOf(this string str, char item, int count = 1)
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
    public static int NthOccuranceFromEnd(this string str, char item, int count = 1)
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

    public static ReadOnlySpan<char> GetNthChunk(this string str, char[] deliminators, int index = 0)
    {
        Guard.IsGreaterThanOrEqualTo(index, 0, nameof(index));

        int start = 0;
        int ind = 0;
        while (index-- >= 0)
        {
            ind = str.IndexOfAny(deliminators, start);
            if (ind == -1)
            {
                // since we've previously decremented
                // index, check against -1;
                // this means we're done.
                if (index == -1)
                {
                    return str.AsSpan().Slice(start);
                }

                // else, we've run out of entries
                // and return an empty span to mark as failure.
                return ReadOnlySpan<char>.Empty;
            }

            if (index > -1)
            {
                start = ind + 1;
            }
        }
        return str.AsSpan()[start..ind];
    }
}