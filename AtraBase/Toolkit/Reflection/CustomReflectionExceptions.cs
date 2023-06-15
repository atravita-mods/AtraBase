using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AtraBase.Toolkit.Reflection;

/// <summary>
/// Thrown when a method accessed by reflection/Harmony isn't found.
/// </summary>
public class MethodNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodNotFoundException"/> class.
    /// </summary>
    /// <param name="methodname">Name of the method.</param>
    public MethodNotFoundException(string methodname)
        : base($"{methodname} not found!")
    {
    }
}

public static class ReflectionThrowHelper
{
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowMethodNotFoundException(string methodName)
    {
        throw new MethodNotFoundException(methodName);
    }

#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowMethodNotFoundException<T>(string methodName)
    {
        throw new MethodNotFoundException(methodName);
    }
}