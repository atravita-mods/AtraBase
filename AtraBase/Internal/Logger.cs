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
        => Console.WriteLine(message);

    /// <inheritdoc />
    public void Error(string message, Exception exception)
        => Console.WriteLine(message + '\n' + exception.ToString());

    /// <inheritdoc />
    public void Info(string message)
        => Console.WriteLine(message);

    /// <inheritdoc />
    public void Verbose(string message)
        => Console.WriteLine(message);

    /// <inheritdoc />
    public void Warn(string message)
        => Console.WriteLine(message);
}
