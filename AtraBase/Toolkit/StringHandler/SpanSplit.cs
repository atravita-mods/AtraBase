﻿namespace AtraBase.Toolkit.StringHandler;

/// <summary>
/// A split entry. Consists of the word + the character split by.
/// (The end of the string is marked with string.Empty).
/// </summary>
public readonly ref struct SpanSplitEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpanSplitEntry"/> struct.
    /// </summary>
    /// <param name="word">Word.</param>
    /// <param name="seperator">Seperator.</param>
    public SpanSplitEntry(ReadOnlySpan<char> word, ReadOnlySpan<char> seperator)
    {
        this.Word = word;
        this.Seperator = seperator;
    }

    /// <summary>
    /// Gets the word.
    /// </summary>
    public ReadOnlySpan<char> Word { get; }

    /// <summary>
    /// Gets the seperator after the word. (String.Empty denotes the end).
    /// </summary>
    public ReadOnlySpan<char> Seperator { get; }

    public static implicit operator ReadOnlySpan<char>(SpanSplitEntry entry) => entry.Word;

    public static implicit operator string(SpanSplitEntry entry) => entry.Word.ToString();

    /// <summary>
    /// Deconstructs the entry.
    /// </summary>
    /// <param name="word">Word.</param>
    /// <param name="seperator">Seperator.</param>
    public void Deconstruct(out ReadOnlySpan<char> word, out ReadOnlySpan<char> seperator)
    {
        word = this.Word;
        seperator = this.Seperator;
    }

    /// <inheritdoc />
    public override string ToString() => this.Word.ToString();
}

/// <summary>
/// A struct that acts as the splitter.
/// </summary>
public ref struct SpanSplit
{
    private readonly char[]? splitchars;
    private readonly StringSplitOptions options;
    private readonly List<(int start, int count, int sepindex)> splitLocs = new();

    private readonly ReadOnlySpan<char> str;
    private ReadOnlySpan<char> remainder;

    private int lastSearchPos = 0;
    private int lastYieldPos = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpanSplit"/> struct.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splitchars">Characters to split by (leave null to mean whitespace).</param>
    /// <param name="options">String split options.</param>
    internal SpanSplit(ReadOnlySpan<char> str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
    {
        this.remainder = this.str = str;
        this.splitchars = splitchars;
        this.options = options;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpanSplit"/> struct.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splitchars">Characters to split by (leave null to mean whitespace).</param>
    /// <param name="options">String split options.</param>
    internal SpanSplit(string str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
    {
        this.remainder = this.str = str.AsSpan();
        this.splitchars = splitchars;
        this.options = options;
    }

    /// <summary>
    /// Gets the total count.
    /// </summary>
    /// <remarks>This will process the whole string.</remarks>
    public int Count
    {
        get
        {
            if (!this.Finished)
            {
                while (this.TryFindNext())
                {
                }
            }
            return this.splitLocs.Count;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this SpanSplit is done processing.
    /// </summary>
    public bool Finished => this.lastSearchPos >= this.str.Length;

    /// <summary>
    /// Indexer.
    /// </summary>
    /// <param name="index">Index.</param>
    /// <returns>A SpanSplitEntry corresponding to the split at that index.</returns>
    /// <exception cref="IndexOutOfRangeException">The index is out of bounds.</exception>
    /// <remarks>Will evaluate more of the string if needed...</remarks>
    public SpanSplitEntry this[int index]
    {
        get
        {
            if (this.TryGetAtIndex(index, out SpanSplitEntry entry))
            {
                return entry;
            }
            else
            {
                throw new IndexOutOfRangeException("Index is out of range!");
            }
        }
    }

    /// <summary>
    /// Tries to get the SpanSplitEntry at the given index.
    /// </summary>
    /// <param name="index">Index to look at.</param>
    /// <param name="entry">SpanSplitEntry.</param>
    /// <returns>True if successful, false otherwise.</returns>
    public bool TryGetAtIndex(int index, out SpanSplitEntry entry)
    {
        if (index < 0)
        {
            entry = default;
            return false;
        }
        if (index >= this.splitLocs.Count)
        {
            while (this.TryFindNext() && index >= this.splitLocs.Count)
            {
            }
        }
        if (index < this.splitLocs.Count)
        {
            (int start, int count, int sep) = this.splitLocs[index];
            entry = new SpanSplitEntry(this.str.Slice(start, count), sep == this.str.Length ? string.Empty : this.str.Slice(sep, 1));
            return true;
        }
        else
        {
            entry = default;
            return false;
        }
    }

    /**********************
     * REGION ENUMERATOR METHODS
     * ********************/

    /// <summary>
    /// Gets the current value - for Enumerator.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Enumerator methods stay together.")]
    public SpanSplitEntry Current { get; private set; } = new SpanSplitEntry(string.Empty, string.Empty);

    /// <summary>
    /// Gets this as an enumerator. Used for ForEach.
    /// </summary>
    /// <returns>this.</returns>
    public SpanSplit GetEnumerator() => this;

    /// <summary>
    /// Moves to the next value.
    /// </summary>
    /// <returns>True if the next value exists, false otherwise.</returns>
    public bool MoveNext()
    {
        this.lastYieldPos++;

        if (this.lastYieldPos >= this.splitLocs.Count)
        {
            while (this.TryFindNext() && this.lastYieldPos >= this.splitLocs.Count)
            {
            }
        }
        if (this.lastYieldPos < this.splitLocs.Count)
        {
            (int start, int count, int sep) = this.splitLocs[this.lastYieldPos];
            this.Current = new SpanSplitEntry(this.str.Slice(start, count), sep == this.str.Length ? string.Empty : this.str.Slice(sep, 1));
            return true;
        }
        return false;
    }

    /// <summary>
    /// Resets the enumerator.
    /// </summary>
    public void Reset() => this.lastYieldPos = -1;

    /***************
     * Private methods.
     * **************/

    /// <summary>
    /// Tries to find the next location to split by.
    /// </summary>
    /// <returns>true if successful, false otherwise.</returns>
    private bool TryFindNext()
    {
        if (this.Finished)
        {
            return false;
        }
        int index;
        int start;
        int end;
        while (true)
        {
            if (this.splitchars is null)
            { // null = we're splitting by whitespace.
                index = -1;
                for (int i = 0; i < this.remainder.Length; i++)
                {
                    if (char.IsWhiteSpace(this.remainder[i]))
                    {
                        index = i;
                        break;
                    }
                }
            }
            else
            {
                index = this.remainder.IndexOfAny(this.splitchars);
            }

            start = this.lastSearchPos;
            if (index < 0)
            { // we've reached the end.
                end = this.str.Length - 1;
                this.remainder = string.Empty;
                this.lastSearchPos = this.str.Length;
            }
            else
            {
                end = this.lastSearchPos + index - 1;
                this.lastSearchPos += index + 1;
                this.remainder = this.str[this.lastSearchPos..];
            }
            if (this.options.HasFlag(StringSplitOptions.TrimEntries))
            {
                (start, end) = this.PerformTrimIfNeeded(start, end);
            }
            if (this.options.HasFlag(StringSplitOptions.RemoveEmptyEntries) && start >= end)
            {
                if (this.lastSearchPos >= this.str.Length)
                {
                    return false;
                }
                else
                {
                    continue;
                }
            }
            this.splitLocs.Add((start, end - start + 1, this.lastSearchPos - 1));
            return true;
        }
    }

    /// <summary>
    /// For StringSplitOptions.Trim, this moves the start and end points to remove whitespace at the start and end.
    /// </summary>
    /// <param name="start">start coordinate.</param>
    /// <param name="end">end coordinate.</param>
    /// <returns>New start and end coordinates.</returns>
    private (int start, int end) PerformTrimIfNeeded(int start, int end)
    {
        while (char.IsWhiteSpace(this.str[start]) && start <= end)
        {
            start++;
        }
        while (char.IsWhiteSpace(this.str[end]) && start <= end)
        {
            end--;
        }
        return (start, end);
    }
}

/// <summary>
/// Holds the extension methods.
/// </summary>
public static class SpanSplitExtension
{
    /// <summary>
    /// Creates a new instance of SpanSplit.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splitchars">Characters to split by. (leave null for whitespace).</param>
    /// <param name="options">String split options.</param>
    /// <returns>SpanSplit instance.</returns>
    public static SpanSplit SpanSplit(this string str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
        => new(str, splitchars, options);

    /// <summary>
    /// Creates a new instance of SpanSplit.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splitchars">Characters to split by. (leave null for whitespace).</param>
    /// <param name="options">String split options.</param>
    /// <returns>SpanSplit instance.</returns>
    public static SpanSplit SpanSplit(this ReadOnlySpan<char> str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
        => new(str, splitchars, options);
}