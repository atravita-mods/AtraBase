using System.Reflection;
using CommunityToolkit.Diagnostics;

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
    public static string GetFullName(this MethodBase method)
        => $"{method.DeclaringType}::{method.Name}";

    /// <summary>
    /// Gets the full name of a MethodInfo.
    /// </summary>
    /// <param name="method">MethodInfo to analyze.</param>
    /// <returns>Fully qualified name of a MethodInfo.</returns>
    [Pure]
    public static string GetFullName(this MethodInfo method)
        => $"{method.DeclaringType}::{method.Name}";
}

/// <summary>
/// Contains extensions against propertyInfos.
/// </summary>
public static class PropertyInfoExtensions
{
    /// <summary>
    /// Indicates whether or not a property is static.
    /// </summary>
    /// <param name="property">Propertyinfo.</param>
    /// <returns>True if static, false otherwise.</returns>
    /// <exception cref="InvalidOperationException">Somehow the property lacks both a getter and a setter. This should never happen.</exception>
    /// <remarks>This is just for completeness - methodinfo and fieldinfo have this, but not propertyinfo.</remarks>
    [Pure]
    public static bool IsStatic(this PropertyInfo property)
    {
        if (!property.CanWrite && !property.CanRead)
        {
            return ThrowHelper.ThrowInvalidOperationException<bool>("This property appears to be mangled.");
        }
        return (property.GetGetMethod(true)?.IsStatic == true) || (property.GetSetMethod(true)?.IsStatic == true);
    }
}