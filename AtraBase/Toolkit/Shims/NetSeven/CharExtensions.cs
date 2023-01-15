/* The following file was mostly copied with minimal changes
 * from https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Char.cs
 * and is licensed MIT by the .NET foundation. */

#pragma warning disable SA1615 // Element return value should be documented
#pragma warning disable SA1611 // Element parameters should be documented

namespace AtraBase.Toolkit.Shims.NetSeven;

/// <summary>
/// Extensions on the char class.
/// Mostly stuff that exists in .net7 copied back to previous versions.
/// </summary>
public static class CharExtensions
{
    /// <summary>Indicates whether a character is within the specified inclusive range.</summary>
    /// <param name="c">The character to evaluate.</param>
    /// <param name="minInclusive">The lower bound, inclusive.</param>
    /// <param name="maxInclusive">The upper bound, inclusive.</param>
    /// <returns>true if <paramref name="c"/> is within the specified range; otherwise, false.</returns>
    /// <remarks>
    /// The method does not validate that <paramref name="maxInclusive"/> is greater than or equal
    /// to <paramref name="minInclusive"/>.  If <paramref name="maxInclusive"/> is less than
    /// <paramref name="minInclusive"/>, the behavior is undefined.
    /// </remarks>
    public static bool IsBetween(char c, char minInclusive, char maxInclusive) =>
        (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="c"/> is an ASCII
    /// character ([ U+0000..U+007F ]).
    /// </summary>
    /// <remarks>
    /// Per http://www.unicode.org/glossary/#ASCII, ASCII is only U+0000..U+007F.
    /// </remarks>
    public static bool IsAscii(char c) => (uint)c <= '\x007f';

    /// <summary>Indicates whether a character is categorized as an ASCII letter.</summary>
    /// <param name="c">The character to evaluate.</param>
    /// <returns>true if <paramref name="c"/> is an ASCII letter; otherwise, false.</returns>
    /// <remarks>
    /// This determines whether the character is in the range 'A' through 'Z', inclusive,
    /// or 'a' through 'z', inclusive.
    /// </remarks>
    public static bool IsAsciiLetter(char c) => (uint)((c | 0x20) - 'a') <= 'z' - 'a';

    /// <summary>Indicates whether a character is categorized as a lowercase ASCII letter.</summary>
    /// <param name="c">The character to evaluate.</param>
    /// <returns>true if <paramref name="c"/> is a lowercase ASCII letter; otherwise, false.</returns>
    /// <remarks>
    /// This determines whether the character is in the range 'a' through 'z', inclusive.
    /// </remarks>
    public static bool IsAsciiLetterLower(char c) => IsBetween(c, 'a', 'z');

    /// <summary>Indicates whether a character is categorized as an uppercase ASCII letter.</summary>
    /// <param name="c">The character to evaluate.</param>
    /// <returns>true if <paramref name="c"/> is a lowercase ASCII letter; otherwise, false.</returns>
    /// <remarks>
    /// This determines whether the character is in the range 'a' through 'z', inclusive.
    /// </remarks>
    public static bool IsAsciiLetterUpper(char c) => IsBetween(c, 'A', 'Z');
}

#pragma warning restore SA1615 // Element return value should be documented
#pragma warning restore SA1611 // Element parameters should be documented