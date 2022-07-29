using AtraBase.Interfaces;

namespace AtraBase.Internal;

/// <inheritdoc />
internal class Logger : ILogger
{
    internal static ILogger Instance = new Logger();

    public void Error(string message)
        => Console.WriteLine(message);

    public void Info(string message)
        => Console.WriteLine(message);

    public void Verbose(string message)
        => Console.WriteLine(message);

    public void Warn(string message)
        => Console.WriteLine(message);
}
