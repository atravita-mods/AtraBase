// Ignore Spelling: splitchar splitchars

using System.Runtime.CompilerServices;

using AtraBase.Toolkit.Extensions;

namespace AtraBase.Toolkit.StringHandler;

/// <summary>
/// Holds extensions for StreamSplit.
/// </summary>
public static class StreamSplitExtensions
{
    /// <summary>
    /// Splits a string by a given character.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splitchar">Character to split by.</param>
    /// <param name="options">String split options.</param>
    /// <returns><see cref="StreamSplit(string, char, StringSplitOptions)"/> instance.</returns>
    public static StreamSplit StreamSplit(this string str, char splitchar, StringSplitOptions options = StringSplitOptions.None)
        => new(str, splitchar, options);

    /// <summary>
    /// Splits a string by one or more characters, or null for whitespace.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splitchars">Characters to split by, or null for whitespace.</param>
    /// <param name="options">String split options.</param>
    /// <returns><see cref="StreamSplit"/> instance.</returns>
    public static StreamSplit StreamSplit(this string str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
        => new(str, splitchars, options);

    /// <summary>
    /// Splits a readonlyspan by a given character.
    /// </summary>
    /// <param name="str">Span to split.</param>
    /// <param name="splitchar">Character to split by.</param>
    /// <param name="options">String split options.</param>
    /// <returns><see cref="StreamSplit(ReadOnlySpan{char}, char, StringSplitOptions)"/> instance.</returns>
    public static StreamSplit StreamSplit(this ReadOnlySpan<char> str, char splitchar, StringSplitOptions options = StringSplitOptions.None)
        => new(str, splitchar, options);

    /// <summary>
    /// Splits a readonlyspan by one or more characters, or null for whitespace.
    /// </summary>
    /// <param name="str">Span to split.</param>
    /// <param name="splitchars">Characters to split by, or null for whitespace.</param>
    /// <param name="options">String split options.</param>
    /// <returns><see cref="StreamSplit"/> instance.</returns>
    public static StreamSplit StreamSplit(this ReadOnlySpan<char> str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
        => new(str, splitchars, options);
}

/// <summary>
/// A struct that tracks the split progress.
/// </summary>
public ref struct StreamSplit
{
    private readonly char[]? splitchars;
    private readonly StringSplitOptions options;
    private ReadOnlySpan<char> remainder;

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamSplit"/> struct.
    /// </summary>
    /// <param name="str">string to split.</param>
    /// <param name="splitchar">character to split by.</param>
    /// <param name="options">split options.</param>
    public StreamSplit(string str, char splitchar, StringSplitOptions options = StringSplitOptions.None)
        : this(str.AsSpan(), [splitchar], options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamSplit"/> struct.
    /// </summary>
    /// <param name="str">string to split.</param>
    /// <param name="splitchars">characters to split by.</param>
    /// <param name="options">split options.</param>
    public StreamSplit(string str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
        : this(str.AsSpan(), splitchars, options )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamSplit"/> struct.
    /// </summary>
    /// <param name="str">span to split.</param>
    /// <param name="splitchar">character to split by.</param>
    /// <param name="options">split options.</param>
    public StreamSplit(ReadOnlySpan<char> str, char splitchar, StringSplitOptions options = StringSplitOptions.None)
        : this(str, [splitchar], options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamSplit"/> struct.
    /// </summary>
    /// <param name="str">span to split.</param>
    /// <param name="splitchars">characters to split by, or null to split by whitespace.</param>
    /// <param name="options">split options.</param>
    public StreamSplit(ReadOnlySpan<char> str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
    {
        this.remainder = str;
        this.splitchars = splitchars;
        this.options = options;
        if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
        {
            this.TrimSplitCharFromStart();

            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                this.remainder = this.remainder.Trim();
            }
        }
    }
    #endregion

    #region enumeratorMethods

    /// <summary>
    /// Gets the current value - for Enumerator.
    /// </summary>
    public SpanSplitEntry Current { get; private set; } = new SpanSplitEntry(string.Empty, string.Empty);

    /// <summary>
    /// Gets the remaining string to process.
    /// </summary>
    public ReadOnlySpan<char> Remainder => this.remainder;

    /// <summary>
    /// Gets this as an enumerator. Used for ForEach.
    /// </summary>
    /// <returns>this.</returns>
    public StreamSplit GetEnumerator() => this;

    /// <summary>
    /// Moves to the next value.
    /// </summary>
    /// <returns>True if the next value exists, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        while (true)
        {
            if (this.remainder.Length == 0)
            {
                return false;
            }
            int index = this.splitchars is null // null corresponds to splitting by any whitespace character.
                ? this.remainder.GetIndexOfWhiteSpace()
                : this.remainder.IndexOfAny(this.splitchars);
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
                word = this.remainder[..index];

                // special case - the windows newline.
                if (this.splitchars is null && this.remainder.Length > index + 2 &&
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
    /// Skips the next N characters.
    /// </summary>
    /// <param name="length">Number of characters to skip.</param>
    internal void Skip(int length) => this.remainder = this.remainder[length..];

    /// <summary>
    /// Trims whitespace characters from the start of the next segment.
    /// </summary>
    internal void TrimStart() => this.remainder = this.remainder.TrimStart();

    #endregion

    #region helpers

    private void TrimSplitCharFromStart()
        => this.remainder = this.splitchars is null ? this.remainder.TrimStart() : this.remainder.TrimStart(this.splitchars);

    #endregion
}
