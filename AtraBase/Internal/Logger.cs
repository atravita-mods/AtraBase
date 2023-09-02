using AtraBase.Interfaces;

namespace AtraBase.Internal;

/// <inheritdoc />
internal class Logger : ILogger
{
    /// <summary>
    /// Gets or sets the logger instance.
    /// </summary>
    internal static ILogger Instance { get; set; } = new Logger();

    /// <inheritdoc />
    public void Error(string message)
    {
#if DEBUG || LOG_ENABLED
        Console.WriteLine(message);
#endif
    }

    /// <inheritdoc />
    public void Error(string message, Exception exception)
    {
#if DEBUG || LOG_ENABLED
        Console.WriteLine(message + '\n' + exception.ToString());
#endif
    }

    /// <inheritdoc />
    public void Info(string message)
    {
#if DEBUG || LOG_ENABLED
        Console.WriteLine(message);
#endif
    }

    /// <inheritdoc />
    public void Verbose(string message)
    {
#if DEBUG || LOG_ENABLED
        Console.WriteLine(message);
#endif
    }

    /// <inheritdoc />
    public void Warn(string message)
    {
#if DEBUG || LOG_ENABLED
        Console.WriteLine(message);
#endif
    }
}
