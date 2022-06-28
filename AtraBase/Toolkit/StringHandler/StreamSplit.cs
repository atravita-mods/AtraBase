using AtraBase.Toolkit.Extensions;

namespace AtraBase.Toolkit.StringHandler;

public static class StreamSplitExtensions
{
    public static StreamSplit StreamSplit(this string str, char splitchar, StringSplitOptions options = StringSplitOptions.None)
        => new(str, splitchar, options);

    public static StreamSplit StreamSplit(this string str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
        => new(str, splitchars, options);

    public static StreamSplit StreamSplit(this ReadOnlySpan<char> str, char splitchar, StringSplitOptions options = StringSplitOptions.None)
        => new(str, splitchar, options);

    public static StreamSplit StreamSplit(this ReadOnlySpan<char> str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
        => new(str, splitchars, options);
}

public ref struct StreamSplit
{
    private readonly char[]? splitchars;
    private readonly StringSplitOptions options;
    private ReadOnlySpan<char> remainder;

    public StreamSplit(string str, char splitchar, StringSplitOptions options = StringSplitOptions.None)
        : this(str.AsSpan(), new[] { splitchar }, options)
    {
    }

    public StreamSplit(string str, char[]? splitchars = null, StringSplitOptions options = StringSplitOptions.None)
        : this(str.AsSpan(), splitchars, options )
    {
    }

    public StreamSplit(ReadOnlySpan<char> str, char splitchar, StringSplitOptions options = StringSplitOptions.None)
        : this(str, new[] { splitchar }, options)
    {
    }

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
                    word = this.remainder[..Math.Max(0, index)];
                    this.remainder = this.remainder[(index + 2)..];
                }
                else
                {
                    splitchar = this.remainder.Slice(index, 1);
                    word = this.remainder[..Math.Max(0, index)];
                    this.remainder = this.remainder[(index + 1)..];
                }
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
