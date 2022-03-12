using System.Runtime.CompilerServices;

namespace AtraBase.Toolkit;

/// <summary>
/// A class that contains useful contants.
/// </summary>
public class TKConstants
{
    /// <summary>
    /// For use when asking the compiler to both inline and aggressively optimize.
    /// </summary>
    public const MethodImplOptions Hot = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
}