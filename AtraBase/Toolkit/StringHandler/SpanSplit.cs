﻿// Ignore Spelling: splitchars splitchar

using System.Runtime.CompilerServices;

using AtraBase.Toolkit.Extensions;

namespace AtraBase.Toolkit.StringHandler;

// TODO: consider the case of splitting by a string?
// Dunno how necessary though...

/// <summary>
/// A struct that acts as the splitter.
/// </summary>
public ref struct SpanSplit
{
    private readonly char[]? splitchars;
    private readonly StringSplitOptions options;
    private readonly List<(int start, int count, int sepindex)> splitLocs;

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
    /// <param name="expectedCount">The expected number of splits.</param>
    public SpanSplit(ReadOnlySpan<char> str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None, int? expectedCount = null)
    {
        if (options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
        {
            str = TrimSplitCharFromStart(str, splitchars);
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                str = str.Trim();
            }
        }

        this.remainder = this.str = str;
        this.splitchars = splitchars;
        this.options = options;
        this.splitLocs = new(expectedCount ?? Math.Min(str.Length / 6, 4));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpanSplit"/> struct.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splitchar">Character to split by.</param>
    /// <param name="options">String split options.</param>
    /// <param name="expectedCount">The expected number of splits.</param>
    public SpanSplit(ReadOnlySpan<char> str, char splitchar, StringSplitOptions options = StringSplitOptions.None, int? expectedCount = null)
        : this(str, new[] { splitchar }, options, expectedCount)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpanSplit"/> struct.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splitchars">Characters to split by (leave null to mean whitespace).</param>
    /// <param name="options">String split options.</param>
    /// <param name="expectedCount">The expected number of splits.</param>
    public SpanSplit(string str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None, int? expectedCount = null)
        : this(str.AsSpan(), splitchars, options, expectedCount)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpanSplit"/> struct.
    /// </summary>
    /// <param name="str">String to split.</param>
    /// <param name="splitchar">Character to split by.</param>
    /// <param name="options">String split options.</param>
    /// <param name="expectedCount">The expected number of splits.</param>
    public SpanSplit(string str, char splitchar, StringSplitOptions options = StringSplitOptions.None, int? expectedCount = null)
        : this(str.AsSpan(), new[] { splitchar }, options, expectedCount)
    {
    }

    /// <summary>
    /// Gets the total count.
    /// </summary>
    /// <remarks>This will process the whole string. <see cref="CountIsAtLeast(int)"></see> may be preferable.</remarks>
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
                TKThrowHelper.ThrowIndexOutOfRangeException();
                return default;
            }
        }
    }

    /// <summary>
    /// Checks to see if the SplitSpan has at least a certain count of elements.
    /// </summary>
    /// <param name="count">Count.</param>
    /// <returns>True/False.</returns>
    /// <remarks>This will process the SplitSpan only as far as necessary.</remarks>
    public bool CountIsAtLeast(int count)
    {
        if (this.splitLocs.Count >= count)
        {
            return true;
        }
        else
        {
            while (this.TryFindNext())
            {
                if (this.splitLocs.Count >= count)
                {
                    return true;
                }
            }
        }
        return false;
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
            goto FAIL;
        }
        while (index >= this.splitLocs.Count && this.TryFindNext())
        {
        }
        if (index < this.splitLocs.Count)
        {
            (int start, int count, int sep) = this.splitLocs[index];
            entry = new SpanSplitEntry(this.str.Slice(start, count), sep == this.str.Length ? string.Empty : this.str.Slice(sep, 1));
            return true;
        }
FAIL:
        entry = default;
        return false;
    }

    /**********************
     * REGION ENUMERATOR METHODS
     * ********************/

    /// <summary>
    /// Gets the current value - for Enumerator.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Keep enumerator members close.")]
    public SpanSplitEntry Current { get; private set; } = default;

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
        bool success = this.TryGetAtIndex(++this.lastYieldPos, out SpanSplitEntry entry);
        if (success)
        {
            this.Current = entry;
        }
        return success;
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
    [MethodImpl(TKConstants.Hot)]
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
            // null = we're splitting by whitespace.
            index = this.splitchars is null ? this.remainder.GetIndexOfWhiteSpace() : this.remainder.IndexOfAny(this.splitchars);

            start = this.lastSearchPos;
            if (index < 0)
            { // we've reached the end.
                end = this.str.Length - 1;
                this.remainder = string.Empty;
                this.lastSearchPos = this.str.Length + 1;
            }
            else
            {
                end = this.lastSearchPos + index - 1;
                this.lastSearchPos += index + 1;
                this.remainder = this.str[this.lastSearchPos..];

                // Remove extra split chars from start.
                if (this.options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
                {
                    this.remainder = TrimSplitCharFromStart(this.remainder, this.splitchars);
                    this.lastSearchPos = this.str.Length - this.remainder.Length;
                }
            }
            if (this.options.HasFlag(StringSplitOptions.TrimEntries))
            {
                this.PerformTrimIfNeeded(ref start, ref end);
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
    private void PerformTrimIfNeeded(ref int start, ref int end)
    {
        while (char.IsWhiteSpace(this.str[start]) && start < end)
        {
            start++;
        }
        while (char.IsWhiteSpace(this.str[end]) && start < end)
        {
            end--;
        }
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Reviewed.")]
    private static ReadOnlySpan<char> TrimSplitCharFromStart(ReadOnlySpan<char> str, char[]? splitchars)
        => splitchars is null ? str.TrimStart() : str.TrimStart(splitchars);
}