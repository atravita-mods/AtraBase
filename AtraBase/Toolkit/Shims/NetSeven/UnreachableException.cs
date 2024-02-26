#if !NET7_0_OR_GREATER

namespace System.Diagnostics;

/// <summary>
/// An exception that should never be thrown.
/// </summary>
public sealed class UnreachableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnreachableException"/> class.
    /// </summary>
    public UnreachableException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnreachableException"/> class.
    /// </summary>
    /// <param name="message">The message that should be attached.</param>
    public UnreachableException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnreachableException"/> class.
    /// </summary>
    /// <param name="message">The message that should be attached.</param>
    /// <param name="innerException">The inner exception.</param>
    public UnreachableException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

#endif