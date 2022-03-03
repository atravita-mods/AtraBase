namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions on numbers.
/// </summary>
internal static class NumberExtensions
{
    /// <summary>
    /// Gets whether or not a float is within a specific margin of another one.
    /// </summary>
    /// <param name="val">First number.</param>
    /// <param name="otherval">Second number.</param>
    /// <param name="margin">Margin.</param>
    /// <returns>True if within the margin, false otherwise.</returns>
    public static bool WithinMargin(this float val, float otherval, float margin = 0.01f)
        => Math.Abs(val - otherval) <= margin;

    /// <summary>
    /// Gets whether or not a float is within a specific margin of another one.
    /// </summary>
    /// <param name="val">First number.</param>
    /// <param name="otherval">Second number.</param>
    /// <param name="margin">Margin.</param>
    /// <returns>True if within the margin, false otherwise.</returns>
    public static bool WithinMargin(this double val, double otherval, double margin = 0.01)
        => Math.Abs(val - otherval) <= margin;

    /// <summary>
    /// Gets whether or not a float is within a specific margin of another one.
    /// </summary>
    /// <param name="val">First number.</param>
    /// <param name="otherval">Second number.</param>
    /// <param name="margin">Margin.</param>
    /// <returns>True if within the margin, false otherwise.</returns>
    public static bool WithinMargin(this decimal val, decimal otherval, decimal margin = 0.01M)
        => Math.Abs(val - otherval) <= margin;
}