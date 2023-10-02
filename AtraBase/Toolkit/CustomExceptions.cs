using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AtraBase.Toolkit;

/// <summary>
/// Thrown when an unexpected enum value is received.
/// </summary>
/// <typeparam name="T">The enum type that received an unexpected value.</typeparam>
public class UnexpectedEnumValueException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedEnumValueException{T}"/> class.
    /// </summary>
    /// <param name="value">The unexpected enum value.</param>
    public UnexpectedEnumValueException(T value)
        : base($"Enum {typeof(T).Name} received unexpected value {value}")
    {
    }
}

/// <summary>
/// Throw helper for these exceptions.
/// </summary>
public static class TKThrowHelper
{
    /// <inheritdoc cref="UnexpectedEnumValueException{T}"/>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowUnexpectedEnumValueException<TEnum>(TEnum value)
    {
        throw new UnexpectedEnumValueException<TEnum>(value);
    }

    /// <inheritdoc cref="UnexpectedEnumValueException{T}"/>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn ThrowUnexpectedEnumValueException<TEnum, TReturn>(TEnum value)
    {
        throw new UnexpectedEnumValueException<TEnum>(value);
    }

    /// <inheritdoc cref="IndexOutOfRangeException"/>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowIndexOutOfRangeException()
    {
        throw new IndexOutOfRangeException();
    }

    /// <inheritdoc cref="IndexOutOfRangeException"/>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn ThrowIndexOutOfRangeException<TReturn>()
    {
        throw new IndexOutOfRangeException();
    }

    /// <inheritdoc cref="InvalidCastException"/>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidCastException()
    {
        throw new InvalidCastException();
    }

    /// <inheritdoc cref="InvalidCastException"/>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn ThrowInvalidCastException<TReturn>()
    {
        throw new InvalidCastException();
    }

    /// <inheritdoc cref="InvalidCastException"/>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidCastException(string message)
    {
        throw new InvalidCastException(message);
    }

    /// <inheritdoc cref="InvalidCastException"/>
#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TReturn ThrowInvalidCastException<TReturn>(string message)
    {
        throw new InvalidCastException(message);
    }
}