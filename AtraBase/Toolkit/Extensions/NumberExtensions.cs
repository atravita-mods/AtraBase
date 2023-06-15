// Ignore Spelling: otherval

using System.Runtime.CompilerServices;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions on numbers.
/// </summary>
public static class NumberExtensions
{
#if !NET6_0_OR_GREATER
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:Statement should not use unnecessary parenthesis", Justification = "Preference.")]
    private static readonly ThreadLocal<Random> _random = new(() => (new Random()).PreWarm());
#endif

    private static Random Random =>
#if NET6_0_OR_GREATER
        Random.Shared;
#else
        _random.Value!;
#endif

    /// <summary>
    /// Gets whether or not a float is within a specific margin of another one.
    /// </summary>
    /// <param name="val">First number.</param>
    /// <param name="otherval">Second number.</param>
    /// <param name="margin">Margin.</param>
    /// <returns>True if within the margin, false otherwise.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
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
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
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
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
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
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int ToIntPrecise(this float val)
        => (int)MathF.Round(val, MidpointRounding.ToEven);

    /// <summary>
    /// Rounds the number to the nearest int.
    /// </summary>
    /// <param name="val">Value to round.</param>
    /// <returns>Integer.</returns>
    /// <remarks>Rounds to even.</remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int ToIntPrecise(this double val)
        => (int)Math.Round(val, MidpointRounding.ToEven);

    /// <summary>
    /// Rounds the number to the nearest int.
    /// </summary>
    /// <param name="val">Value to round.</param>
    /// <returns>Integer.</returns>
    /// <remarks>Rounds to even.</remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
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

    /// <summary>
    /// Round proportional ie - 7.4 has a 40% chance of becoming 8 and a 60% chance of becoming 7.
    /// </summary>
    /// <param name="val">VAlue to round.</param>
    /// <returns>Int.</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int RandomRoundProportional(this float val)
    {
        double below = Math.Floor(val);
        return Random.NextDouble() < (val - below) ? (int)(below + 1) : (int)below;
    }

    /// <summary>
    /// Round proportional ie - 7.4 has a 40% chance of becoming 8 and a 60% chance of becoming 7.
    /// </summary>
    /// <param name="val">VAlue to round.</param>
    /// <returns>Int.</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int RandomRoundProportional(this double val)
    {
        double below = Math.Floor(val);
        return Random.NextDouble() < (val - below) ? (int)(below + 1) : (int)below;
    }

    /// <summary>
    /// Round proportional ie - 7.4 has a 40% chance of becoming 8 and a 60% chance of becoming 7.
    /// </summary>
    /// <param name="val">VAlue to round.</param>
    /// <returns>Int.</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int RandomRoundProportional(this decimal val)
    {
        decimal below = Math.Floor(val);
        return (decimal)Random.NextDouble() < (val - below) ? (int)(below + 1) : (int)below;
    }
}