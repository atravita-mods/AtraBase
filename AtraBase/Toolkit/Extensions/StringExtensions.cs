﻿using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extension methods on strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Gets the Nth occurrence of a specific Unicode char in a string.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <param name="item">Char to search for.</param>
    /// <param name="count">N.</param>
    /// <returns>Index of the char, or -1 if not found.</returns>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
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
    /// Gets the Nth occurrence from the end of a specific Unicode char in a string.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <param name="item">Char to search for.</param>
    /// <param name="count">N.</param>
    /// <returns>Index of the char, or -1 if not found.</returns>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
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

    /// <summary>
    /// Faster replacement for str.Split()[index];.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <param name="deliminator">deliminator to use.</param>
    /// <param name="index">index of the chunk to get.</param>
    /// <returns>a readonlyspan char with the chunk, or an empty readonlyspan for failure.</returns>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static ReadOnlySpan<char> GetNthChunk(this string str, char deliminator, int index = 0)
    {
        Guard.IsBetweenOrEqualTo(index, 0, str.Length + 1);

        int start = 0;
        int ind = 0;
        while (index-- >= 0)
        {
            ind = str.IndexOf(deliminator, start);
            if (ind == -1)
            {
                // since we've previously decremented index, check against -1;
                // this means we're done.
                if (index == -1)
                {
                    return str.AsSpan()[start..];
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

    /// <summary>
    /// Faster replacement for str.Split()[index];.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <param name="deliminators">deliminators to use.</param>
    /// <param name="index">index of the chunk to get.</param>
    /// <returns>a readonlyspan char with the chunk, or an empty readonlyspan for failure.</returns>
    /// <remarks>Inspired by the lovely Wren.</remarks>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static ReadOnlySpan<char> GetNthChunk(this string str, char[] deliminators, int index = 0)
    {
        Guard.IsBetweenOrEqualTo(index, 0, str.Length + 1);

        int start = 0;
        int ind = 0;
        while (index-- >= 0)
        {
            ind = str.IndexOfAny(deliminators, start);
            if (ind == -1)
            {
                // since we've previously decremented index, check against -1;
                // this means we're done.
                if (index == -1)
                {
                    return str.AsSpan()[start..];
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

    /// <summary>
    /// Gets the index of the next whitespace character.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <returns>Index of the whitespace character, or -1 if not found.</returns>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static int GetIndexOfWhiteSpace(this string str)
        => str.AsSpan().GetIndexOfWhiteSpace();

    /// <summary>
    /// Gets the index of the next whitespace character.
    /// </summary>
    /// <param name="chars">ReadOnlySpan to search in.</param>
    /// <returns>Index of the whitespace character, or -1 if not found.</returns>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static int GetIndexOfWhiteSpace(this ReadOnlySpan<char> chars)
    {
        for (int i = 0; i < chars.Length; i++)
        {
            if (char.IsWhiteSpace(chars[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Gets the index of the last whitespace character.
    /// </summary>
    /// <param name="str">String to search in.</param>
    /// <returns>Index of the whitespace character, or -1 if not found.</returns>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static int GetLastIndexOfWhiteSpace(this string str)
        => str.AsSpan().GetLastIndexOfWhiteSpace();

    /// <summary>
    /// Gets the index of the last whitespace character.
    /// </summary>
    /// <param name="chars">ReadOnlySpan to search in.</param>
    /// <returns>Index of the whitespace character, or -1 if not found.</returns>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static int GetLastIndexOfWhiteSpace(this ReadOnlySpan<char> chars)
    {
        for (int i = chars.Length - 1; i >= 0; i--)
        {
            if (char.IsWhiteSpace(chars[i]))
            {
                return i;
            }
        }
        return -1;
    }

    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static bool TrySplitOnce(this string str, char? deliminator, out ReadOnlySpan<char> first, out ReadOnlySpan<char> second)
    {
        Guard.IsNotNull(str);
        return str.AsSpan().TrySplitOnce(deliminator, out first, out second);
    }

    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static bool TrySplitOnce(this ReadOnlySpan<char> str, char? deliminator, out ReadOnlySpan<char> first, out ReadOnlySpan<char> second)
    {
        int idx = deliminator is null ? str.GetIndexOfWhiteSpace() : str.IndexOf(deliminator.Value);

        if (idx < 0)
        {
            first = second = ReadOnlySpan<char>.Empty;
            return false;
        }

        first = str[..idx];
        second = str[(idx + 1)..];
        return true;
    }
}