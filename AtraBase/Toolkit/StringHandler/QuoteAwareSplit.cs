using System.Diagnostics;
using System.Runtime.CompilerServices;

using AtraBase.Toolkit.Extensions;

namespace AtraBase.Toolkit.StringHandler;

/// <summary>
/// A quote-aware split engine.
/// </summary>
public ref struct QuoteAwareSplit
{
    private readonly char[]? searchchars = null;
    private readonly char[]? splitchars = null;
    private readonly char quoteChar = '"';
    private readonly char escapeChar = '\\';
    private readonly StringSplitOptions options;
    private readonly bool keepQuotesAndEscapes = false;
    private ReadOnlySpan<char> remainder;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteAwareSplit"/> struct.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splitchars">Character to split by, or null for whitespace.</param>
    /// <param name="quoteChar">The character we will consider a quote.</param>
    /// <param name="escapeChar">The character we will consider an escape.</param>
    /// <param name="options">The string split options to use.</param>
    /// <param name="keepQuotesAndEscapes">Whether or not we should preserve the quote characters.</param>
    public QuoteAwareSplit(ReadOnlySpan<char> str, char[]? splitchars, char quoteChar = '"', char escapeChar = '\\', StringSplitOptions options = StringSplitOptions.None, bool keepQuotesAndEscapes = false)
    {
        // sanity checks.
        Debug.Assert(splitchars is null ? !char.IsWhiteSpace(quoteChar) : !splitchars.Contains(quoteChar), $"quoteChar {quoteChar} cannot be a valid splitChar");
        Debug.Assert(splitchars is null ? !char.IsWhiteSpace(escapeChar) : !splitchars.Contains(escapeChar), $"escapeChar {escapeChar} cannot be a valid splitChar");
        // also check if either quote or splitchar is a whitespace char if trimming is enabled?

        if (splitchars is not null)
        {
            // we sneak the split and escape characters into the splitchars so we can
            // toss the whole thing into IndexOfAny
            // This is a heap allocation, but a teensy one that simplifies our logic by a lot.
            // Also why I'm avoiding Array.Copy here.
            var searchchars = new char[splitchars.Length + 2];
            for (int i = 0; i < splitchars.Length; i++)
            {
                searchchars[i] = splitchars[i];
            }
            searchchars[^1] = quoteChar;
            searchchars[^2] = escapeChar;

            this.searchchars = searchchars;
            this.splitchars = splitchars;
        }

        this.quoteChar = quoteChar;
        this.escapeChar = escapeChar;
        this.options = options;
        this.keepQuotesAndEscapes = keepQuotesAndEscapes;
        this.remainder = str;

        if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
        {
            this.TrimSplitCharFromStart();

            // if either quoteChar or escapeChar is a whitespace character (for the love of god don't)
            // I can't just trim at the start to save me some time.
            if (options.HasFlag(StringSplitOptions.TrimEntries)
                && !char.IsWhiteSpace(quoteChar) && !char.IsWhiteSpace(escapeChar))
            {
                this.remainder = this.remainder.Trim();
            }
        }
    }

    #region enumerator methods

    /// <summary>
    /// Gets the current value - for Enumerator.
    /// </summary>
    public SpanSplitEntry Current { get; private set; } = new SpanSplitEntry(string.Empty, string.Empty);

    /// <summary>
    /// Gets the remaining string to process.
    /// </summary>
    public readonly ReadOnlySpan<char> Remainder => this.remainder;

    /// <summary>
    /// Gets this as an enumerator. Used for ForEach.
    /// </summary>
    /// <returns>this.</returns>
    public QuoteAwareSplit GetEnumerator() => this;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        while (true)
        {
            if (this.remainder.Length == 0)
            {
                return false;
            }

            int index = this.splitchars is null // null corresponds to splitting by any whitespace character
                ? this.remainder.GetIndexOfWhiteSpaceOr(this.quoteChar, this.escapeChar)
                : this.remainder.IndexOfAny(this.searchchars);

            ReadOnlySpan<char> splitchar;
            ReadOnlySpan<char> word;

            if (index < 0)
            {
                splitchar = string.Empty;
                word = this.remainder;
                this.remainder = string.Empty;
            }
            else
            {
                char found = this.remainder[index];
                if (found == this.quoteChar || found == this.escapeChar)
                {
                    index = this.keepQuotesAndEscapes
                        ? this.ProcessQuoteAwareSplitKeep(this.remainder, index, found, out word)
                        : this.ProcessQuoteAwareSplitStrip(this.remainder, index, found, out word);
                }
                else
                {
                    word = this.remainder[..index];
                }

                // branch predictor halp.
                if (index < 0)
                {
                    splitchar = string.Empty;
                    this.remainder = string.Empty;
                }
                else if (this.splitchars is null && this.remainder.Length > index + 2 &&
                    this.remainder.Slice(index, 2).Equals("\r\n", StringComparison.Ordinal))
                {
                    splitchar = this.remainder.Slice(index, 2);
                    this.remainder = this.remainder[(index + 2)..];
                }
                else
                {
                    splitchar = this.remainder.Slice(index, 1);
                    this.remainder = this.remainder[(index + 1)..];
                }
            }

            if (this.options.HasFlag(StringSplitOptions.TrimEntries))
            {
                word = word.Trim();
            }
            if (this.options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                this.TrimSplitCharFromStart();
                if (word.Length == 0)
                {
                    continue;
                }
            }
            this.Current = new SpanSplitEntry(word, splitchar);
            return true;
        }
    }

    /// <summary>
    /// Finds the next valid split location taking into account <see cref="quoteChar"/> and <see cref="escapeChar"/>, also returning the <paramref name="word"/> stripped of those characters.
    /// </summary>
    /// <param name="segment">The segment to search in.</param>
    /// <param name="index">The index of either the escape or quote character.</param>
    /// <param name="found">Which of the characters was found.</param>
    /// <param name="word">The gene</param>
    /// <returns>The index of the next valid split position, or -1 for not found.</returns>
    private int ProcessQuoteAwareSplitStrip(ReadOnlySpan<char> segment, int index, char found, out ReadOnlySpan<char> word)
    {
        // if we need to remove the quotes and escape characters
        // we'll need to build the string fragment somewhere.
        // let's try our best to use VSB.
        var buffer = new ValueStringBuilder(stackalloc char[256]);

        word = string.Empty;
        buffer.Append(segment[..index]);
        throw new NotImplementedException();
    }

    private int ProcessQuoteAwareSplitKeep(ReadOnlySpan<char> segment, int index, char found, out ReadOnlySpan<char> word)
    {
        // in a real way this is the easier of the two, since I don't need to build the string again in a buffer.

        word = string.Empty;
        throw new NotImplementedException();
    }

    #endregion

    #region helpers
    private void TrimSplitCharFromStart()
        => this.remainder = this.splitchars is null ? this.remainder.TrimStart() : this.remainder.TrimStart(this.splitchars);
    #endregion
}

file static class QuoteAwareSplitExtensions
{
    internal static int GetIndexOfWhiteSpaceOr(this ReadOnlySpan<char> chars, char first, char second)
    {
        for (int i = 0; i < chars.Length; i++)
        {
            var c = chars[i];
            if (c == first || c == second || char.IsWhiteSpace(c))
            {
                return i;
            }
        }
        return -1;
    }
}