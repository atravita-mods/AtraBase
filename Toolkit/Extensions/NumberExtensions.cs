using System.Runtime.CompilerServices;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions on numbers.
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    /// Gets whether or not a float is within a specific margin of another one.
    /// </summary>
    /// <param name="val">First number.</param>
    /// <param name="otherval">Second number.</param>
    /// <param name="margin">Margin.</param>
    /// <returns>True if within the margin, false otherwise.</returns>
    [Pure]
    public static bool WithinMargin(this float val, float otherval, float margin = 0.01f)
        => Math.Abs(val - otherval) <= margin;

    /// <summary>
    /// Gets whether or not a float is within a specific margin of another one.
    /// </summary>
    /// <param name="val">First number.</param>
    /// <param name="otherval">Second number.</param>
    /// <param name="margin">Margin.</param>
    /// <returns>True if within the margin, false otherwise.</returns>
    [Pure]
    public static bool WithinMargin(this double val, double otherval, double margin = 0.01)
        => Math.Abs(val - otherval) <= margin;

    /// <summary>
    /// Gets whether or not a float is within a specific margin of another one.
    /// </summary>
    /// <param name="val">First number.</param>
    /// <param name="otherval">Second number.</param>
    /// <param name="margin">Margin.</param>
    /// <returns>True if within the margin, false otherwise.</returns>
    [Pure]
    public static bool WithinMargin(this decimal val, decimal otherval, decimal margin = 0.01M)
        => Math.Abs(val - otherval) <= margin;

    // TODO: benchmark these?

    /// <summary>
    /// Rounds the number to the nearest int.
    /// </summary>
    /// <param name="val">Value to round.</param>
    /// <returns>Integer.</returns>
    /// <remarks>Rounds to even.</remarks>
    [Pure]
    public static int ToIntPrecise(this float val)
        => (int)MathF.Round(val, MidpointRounding.ToEven);

    /// <summary>
    /// Rounds the number to the nearest int.
    /// </summary>
    /// <param name="val">Value to round.</param>
    /// <returns>Integer.</returns>
    /// <remarks>Rounds to even.</remarks>
    [Pure]
    public static int ToIntPrecise(this double val)
        => (int)Math.Round(val, MidpointRounding.ToEven);

    /// <summary>
    /// Rounds the number to the nearest int.
    /// </summary>
    /// <param name="val">Value to round.</param>
    /// <returns>Integer.</returns>
    /// <remarks>Rounds to even.</remarks>
    [Pure]
    public static int ToIntPrecise(this decimal val)
        => (int)Math.Round(val, MidpointRounding.ToEven);

    /// <summary>
    /// Rounds the number to the nearest int.
    /// </summary>
    /// <param name="val">Value to round.</param>
    /// <returns>Integer.</returns>
    /// <remarks>Rounding method not precisely defined.</remarks>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static int ToIntFast(this float val)
        => (int)(val + 0.5f);

    /// <summary>
    /// Rounds the number to the nearest int.
    /// </summary>
    /// <param name="val">Value to round.</param>
    /// <returns>Integer.</returns>
    /// <remarks>Rounding method not precisely defined.</remarks>
    [Pure]
    [MethodImpl(TKConstants.Hot)]
    public static int ToIntFast(this double val)
        => (int)(val + 0.5d);

    // No point doing a ToIntFast for Decimal.
}