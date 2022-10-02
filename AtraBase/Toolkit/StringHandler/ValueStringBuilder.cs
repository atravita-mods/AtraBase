/* The following file was mostly copied with minimal changes
 * from https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/Text/ValueStringBuilder.cs
 * and is licensed MIT by the .NET foundation.
 * Any additional code has been marked.*/

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using CommunityToolkit.Diagnostics;

#if !NET7_0_OR_GREATER
using AtraBase.Toolkit.Shims.NetSeven;
#endif

namespace AtraBase.Toolkit.StringHandler;

#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1201 // Elements should appear in the correct order

/// <summary>
/// A variant of StringBuilder that tries not to allocate.
/// Usage notes: **always** pass by ref. Best used when the
/// final capacity is known beforehand.
/// Growing is very expensive.
/// </summary>
internal ref struct ValueStringBuilder
{
    private char[]? _arrayToReturnToPool;
    private Span<char> _chars;
    private int _pos;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="initialBuffer">The initial buffer to use.</param>
    public ValueStringBuilder(Span<char> initialBuffer)
    {
        this._arrayToReturnToPool = null;
        this._chars = initialBuffer;
        this._pos = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="initialCapacity">Initial capacity.</param>
    /// <remarks>Always rents.</remarks>
    public ValueStringBuilder(int initialCapacity)
    {
        this._arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        this._chars = this._arrayToReturnToPool;
        this._pos = 0;
    }

    public int Length
    {
        get => this._pos;
    }

    public int Capacity => this._chars.Length;

    public void EnsureCapacity(int capacity)
    {
        // This is not expected to be called this with negative capacity
        Guard.IsGreaterThanOrEqualTo(capacity, 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if ((uint)capacity > (uint)this._chars.Length)
            this.Grow(capacity - this._pos);
    }

    public ref char this[int index]
    {
        get
        {
            Guard.IsBetween(index, 0, this._pos);
            return ref this._chars[index];
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string s = this._chars[..this._pos].ToString();
        this.Dispose();
        return s;
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<char> RawChars => this._chars;

    public ReadOnlySpan<char> AsSpan() => this._chars[..this._pos];
    public ReadOnlySpan<char> AsSpan(int start) => this._chars[start..this._pos];
    public ReadOnlySpan<char> AsSpan(int start, int length) => this._chars.Slice(start, length);

    /// <summary>
    /// Tries to copy the value in the VSB to the destination buffer.
    /// </summary>
    /// <param name="destination">Destination buffer.</param>
    /// <param name="charsWritten">Number of characters copied</param>
    /// <returns>Whether the copy operation was successful.</returns>
    /// <remarks>Unlike the standard implementation, does not dispose the VSB.</remarks>
    public bool TryCopyTo(Span<char> destination, out int charsWritten)
    {
        if (this._chars[..this._pos].TryCopyTo(destination))
        {
            charsWritten = this._pos;
            return true;
        }
        else
        {
            charsWritten = 0;
            return false;
        }
    }

    public void Insert(int index, char value, int count)
    {
        if (this._pos > this._chars.Length - count)
        {
            this.Grow(count);
        }

        int remaining = this._pos - index;
        this._chars.Slice(index, remaining).CopyTo(this._chars[(index + count)..]);
        this._chars.Slice(index, count).Fill(value);
        this._pos += count;
    }

    public void Insert(int index, string? s)
    {
        if (s == null)
        {
            return;
        }

        int count = s.Length;

        if (this._pos > (this._chars.Length - count))
        {
            this.Grow(count);
        }

        int remaining = this._pos - index;
        this._chars.Slice(index, remaining).CopyTo(this._chars[(index + count)..]);
        s
#if !NET6_0_OR_GREATER
            .AsSpan()
#endif
            .CopyTo(this._chars[index..]);
        this._pos += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        int pos = this._pos;
        if ((uint)pos < (uint)this._chars.Length)
        {
            this._chars[pos] = c;
            this._pos = pos + 1;
        }
        else
        {
            this.GrowAndAppend(c);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? s)
    {
        if (s == null)
        {
            return;
        }

        int pos = this._pos;
        if (s.Length == 1 && (uint)pos < (uint)this._chars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
        {
            this._chars[pos] = s[0];
            this._pos = pos + 1;
        }
        else
        {
            this.AppendSlow(s);
        }
    }

    private void AppendSlow(string s)
    {
        int pos = this._pos;
        if (pos > this._chars.Length - s.Length)
        {
            this.Grow(s.Length);
        }

        s
#if !NET6_0_OR_GREATER
            .AsSpan()
#endif
            .CopyTo(this._chars[pos..]);
        this._pos += s.Length;
    }

    public void Append(char c, int count)
    {
        if (this._pos > this._chars.Length - count)
        {
            this.Grow(count);
        }

        Span<char> dst = this._chars.Slice(this._pos, count);
        for (int i = 0; i < dst.Length; i++)
        {
            dst[i] = c;
        }
        this._pos += count;
    }

    public void Append(ReadOnlySpan<char> value)
    {
        int pos = this._pos;
        if (pos > this._chars.Length - value.Length)
        {
            this.Grow(value.Length);
        }

        value.CopyTo(this._chars[this._pos..]);
        this._pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        int origPos = this._pos;
        if (origPos > this._chars.Length - length)
        {
            this.Grow(length);
        }

        this._pos = origPos + length;
        return this._chars.Slice(origPos, length);
    }

    #region AddedMethods

    /// <summary>
    /// Lowercases all the ascii letters in this VSB.
    /// </summary>
    public void ToLowerAscii()
    {
        for (int i = 0; i < this._pos; i++)
        {
            if (CharExtensions.IsAsciiLetterUpper(this._chars[i]))
            {
                this._chars[i] = (char)(this._chars[i] | 0x20);
            }
        }
    }

    /// <summary>
    /// Uppercasses all the ascii letters in this VSB.
    /// </summary>
    public void ToUpperAscii()
    {
        for (int i = 0; i < this._pos; i++)
        {
            if (CharExtensions.IsAsciiLetterLower(this._chars[i]))
            {
                this._chars[i] = (char)(this._chars[i] & ~0x20);
            }
        }
    }

    #endregion

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        this.Grow(1);
        this.Append(c);
    }

    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Guard.IsGreaterThan(additionalCapacityBeyondPos, 0);
        Debug.Assert(this._pos > this._chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative
        char[] poolArray = ArrayPool<char>.Shared.Rent((int)Math.Max((uint)(this._pos + additionalCapacityBeyondPos), (uint)this._chars.Length * 2));

        this._chars[..this._pos].CopyTo(poolArray);

        char[]? toReturn = this._arrayToReturnToPool;
        this._chars = this._arrayToReturnToPool = poolArray;
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    /// <summary>
    /// Returns the rented array if any, sets all fields to default.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        char[]? toReturn = this._arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }
}

#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1201 // Elements should appear in the correct order