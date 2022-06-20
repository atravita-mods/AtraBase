﻿using System.Runtime.CompilerServices;

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

internal static partial class TKThrowHelper
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowMethodNotFoundException(string methodName)
    {
        throw new MethodNotFoundException(methodName);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static T ThrowMethodNotFoundException<T>(string methodName)
    {
        throw new MethodNotFoundException(methodName);
    }
}