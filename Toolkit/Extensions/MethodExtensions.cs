using System.Reflection;
using System.Runtime.CompilerServices;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Small extensions to get the full name of a method.
/// </summary>
public static class MethodExtensions
{
    /// <summary>
    /// Gets the full name of a MethodBase.
    /// </summary>
    /// <param name="method">MethodBase to analyze.</param>
    /// <returns>Fully qualified name of a MethodBase.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetFullName(this MethodBase method)
        => $"{method.DeclaringType}::{method.Name}";

    /// <summary>
    /// Gets the full name of a MethodInfo.
    /// </summary>
    /// <param name="method">MethodInfo to analyze.</param>
    /// <returns>Fully qualified name of a MethodInfo.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetFullName(this MethodInfo method)
        => $"{method.DeclaringType}::{method.Name}";
}