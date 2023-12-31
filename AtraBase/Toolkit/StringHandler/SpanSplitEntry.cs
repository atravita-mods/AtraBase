namespace AtraBase.Toolkit.StringHandler;

/// <summary>
/// A split entry. Consists of the word + the character split by.
/// (The end of the string is marked with string.Empty).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SpanSplitEntry"/> struct.
/// </remarks>
/// <param name="word">Word.</param>
/// <param name="separator">Separator.</param>
public readonly ref struct SpanSplitEntry(ReadOnlySpan<char> word, ReadOnlySpan<char> separator)
{
    /// <summary>
    /// Gets the word.
    /// </summary>
    public ReadOnlySpan<char> Word { get; } = word;

    /// <summary>
    /// Gets the separator after the word. (String.Empty denotes the end).
    /// </summary>
    public ReadOnlySpan<char> Separator { get; } = separator;

    /// <summary>
    /// Converts this entry to a ReadOnlySpan.
    /// </summary>
    /// <param name="entry">Entry to convert.</param>
    public static implicit operator ReadOnlySpan<char>(SpanSplitEntry entry) => entry.Word;

    /// <summary>
    /// Converts the entry to a string.
    /// </summary>
    /// <param name="entry">Entry to convert.</param>
    public static implicit operator string(SpanSplitEntry entry) => entry.Word.ToString();

    /// <summary>
    /// Deconstructs the entry.
    /// </summary>
    /// <param name="word">Word.</param>
    /// <param name="separator">Separator.</param>
    public void Deconstruct(out ReadOnlySpan<char> word, out ReadOnlySpan<char> separator)
    {
        word = this.Word;
        separator = this.Separator;
    }

    /// <summary>
    /// Whether this entry contains the string.
    /// </summary>
    /// <param name="str">Substring.</param>
    /// <param name="comparison">The comparsion method - defaults to ordinal.</param>
    /// <returns>True if this entry contains that string.</returns>
    public bool Contains(ReadOnlySpan<char> str, StringComparison comparison = StringComparison.Ordinal)
        => this.Word.Contains(str, comparison);

    /// <summary>
    /// Whether this entry starts with the string.
    /// </summary>
    /// <param name="str">Substring.</param>
    /// <param name="comparison">The comparison method - defaults to ordinal.</param>
    /// <returns>True if this entry starts with that string.</returns>
    public bool StartsWith(ReadOnlySpan<char> str, StringComparison comparison = StringComparison.Ordinal)
        => this.Word.StartsWith(str, comparison);

    /// <summary>
    /// Whether or not this entry ends with a specific string.
    /// </summary>
    /// <param name="str">Substring.</param>
    /// <param name="comparison">The comparison method - defaults to ordinal.</param>
    /// <returns>True if this entry ends with that string.</returns>
    public bool EndsWith(ReadOnlySpan<char> str, StringComparison comparison = StringComparison.Ordinal)
        => this.Word.EndsWith(str, comparison);

    /// <inheritdoc />
    public override string ToString() => this.Word.ToString();
}