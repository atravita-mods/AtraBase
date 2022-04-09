namespace AtraBase.Toolkit.StringHandler;

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

    /// <summary>
    /// Whether this entry contains the string.
    /// </summary>
    /// <param name="str">Substring.</param>
    /// <returns>True if this entry contains that string.</returns>
    public bool Contains(string str)
    {
        for (int i = 0; i < this.Word.Length - str.Length + 1; i++)
        {
            for (int j = 0; j < str.Length; j++)
            {
                if (!this.Word[i + j].Equals(str[j]))
                {
                    goto ContainsLoopContinue;
                }
            }
            return true;
ContainsLoopContinue:;
        }
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => this.Word.ToString();
}