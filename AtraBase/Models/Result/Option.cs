using CommunityToolkit.Diagnostics;

namespace AtraBase.Models.Result;

/// <summary>
/// Based of Rust's Option struct.
/// </summary>
public struct Option<T>
{
    private T? value;
    private bool isNotNone = false; // `default` inits all fields to **their** default, so doing this "backwards" to make `default` work.

    /// <summary>
    /// Initializes a new instance of the <see cref="Option{T}"/> struct,
    /// representing Some(value).
    /// </summary>
    /// <param name="value">Value to represent.</param>
    public Option(T? value)
    {
        this.value = value;
        this.isNotNone = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Option{T}"/> struct,
    /// representing no value (ie, Rust's None).
    /// </summary>
    public Option() => this.isNotNone = false;

    private static Option<T> noneInstance = new();
    public static Option<T> None => noneInstance;

    public bool IsNone => !this.isNotNone;

    public bool TryGetValue(out T? value)
    {
        if (this.isNotNone)
        {
            value = this.value;
            return true;
        }
        value = default;
        return false;
    }

    public T? Unwrap()
    {
        if (this.isNotNone)
        {
            return this.value;
        }
        return ThrowHelper.ThrowInvalidDataException<T>("Panic! This is None :(.");
    }

    public T? Unwrap_Or(T? @default)
        => this.isNotNone ? this.value : @default;

    public T? Unwrap_Or_Default()
    {
        if (this.isNotNone)
        {
            return this.value;
        }
        return default;
    }

    public T? Unwrap_Or_Else(Func<T?> del)
        => this.isNotNone ? this.value : del();
}
