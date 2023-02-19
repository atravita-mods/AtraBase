using CommunityToolkit.Diagnostics;

namespace AtraBase.Models.Result;

/// <summary>
/// Based of Rust's Option struct.
/// </summary>
public struct Option<T>
{
    private static readonly Option<T> noneInstance = new();

    private readonly T? value;
    private readonly bool isNotNone = false; // `default` inits all fields to **their** default, so doing this "backwards" to make `default` work.

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

    /// <summary>
    /// Gets a singleton for None.
    /// </summary>
    public static Option<T> None => noneInstance;

    /// <summary>
    /// Gets a value indicating whether or not this instance represents a None value.
    /// </summary>
    public bool IsNone => !this.isNotNone;

    /// <summary>
    /// Tries to get the internal value.
    /// </summary>
    /// <param name="value">Value if this instance is not None.</param>
    /// <returns>True if this instance is not None, false otherwise.</returns>
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

    /// <summary>
    /// Gets the value, or throws an error otherwise.
    /// </summary>
    /// <returns>Value.</returns>
    /// <exception cref="InvalidDataException">Throw if this is called on None.</exception>
    public T? Unwrap()
    {
        if (this.isNotNone)
        {
            return this.value;
        }
        return ThrowHelper.ThrowInvalidDataException<T>("Panic! This is None :(.");
    }

    /// <summary>
    /// Gets the value if this is not a None instance, or substitutes the default value given.
    /// </summary>
    /// <param name="default">The default value.</param>
    /// <returns>The value, if there is one, or the default value.</returns>
    public T? Unwrap_Or(T? @default)
        => this.isNotNone ? this.value : @default;

    /// <summary>
    /// Gets the value of if this isn't a None instance, or uses the default for T.
    /// </summary>
    /// <returns>Value, or the default value for the type.</returns>
    public T? Unwrap_Or_Default()
    {
        if (this.isNotNone)
        {
            return this.value;
        }
        return default;
    }

    /// <summary>
    /// Gets the value if this isn't a None instance, or uses the provided delegate to give a default value.
    /// </summary>
    /// <param name="del">Delegate that provides a default value.</param>
    /// <returns>Value, or the value from the delegate if this was a None instance.</returns>
    public T? Unwrap_Or_Else(Func<T?> del)
        => this.isNotNone ? this.value : del();
}
