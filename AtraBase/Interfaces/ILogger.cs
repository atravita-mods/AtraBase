namespace AtraBase.Interfaces;

/// <summary>
/// Encapsulates Atrabase logging.
/// </summary>
internal interface ILogger
{
    internal void Verbose(string message);

    internal void Info(string message);

    internal void Warn(string message);

    internal void Error(string message);
}
