using AtraBase.Toolkit.Extensions;

namespace AtraBase.Toolkit.StringHandler;

public ref struct StreamSplit
{
    private readonly char[]? splitchars;
    private readonly StringSplitOptions options;
    private ReadOnlySpan<char> remainder;

    public StreamSplit(ReadOnlySpan<char> str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
    {
        this.remainder = str;
        this.splitchars = splitchars;
        this.options = options;
    }

    /***************
     * REGION ENUMERATOR METHODS
     * *************/

    public SpanSplitEntry Current { get; private set; } = new SpanSplitEntry(string.Empty, string.Empty);

    public StreamSplit GetEnumerator() => this;

    public bool MoveNext()
    {
        while (true)
        {
            if (this.remainder.Length == 0)
            {
                return false;
            }
            int index;
            if (this.splitchars is null)
            { // we're splitting by whitespace
                index = this.remainder.GetIndexOfWhiteSpace();
            }
            else
            {
                index = this.remainder.IndexOfAny(this.splitchars);
            }
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
                word = this.remainder[..(index - 1)];
            }
            

            if (this.options.HasFlag(StringSplitOptions.TrimEntries))
            {
                word = word.Trim();
            }
            if (this.options.HasFlag(StringSplitOptions.RemoveEmptyEntries) & word.Length == 0)
            {
                continue;
            }
            this.Current = new SpanSplitEntry(word, splitchar);
            return true;
        }

    }
}
