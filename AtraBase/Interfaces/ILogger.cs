namespace AtraBase.Interfaces;

/// <summary>
/// Encapsulates Atrabase logging.
/// </summary>
internal interface ILogger
{
    /// <summary>
    /// Logs a message. Meant for detailed messages that usually are not important.
    /// </summary>
    /// <param name="message">Message to log.</param>
    internal void Verbose(string message);

    /// <summary>
    /// Logs a message. Meant for useful details that do not require action.
    /// </summary>
    /// <param name="message">Message to log.</param>
    internal void Info(string message);

    /// <summary>
    /// Logs a message. Use for when action should be taken.
    /// </summary>
    /// <param name="message">Message to log.</param>
    internal void Warn(string message);

    /// <summary>
    /// Logs an error. Something has gone wrong.
    /// </summary>
    /// <param name="message">Message to log.</param>
    internal void Error(string message);

    /// <summary>
    /// Logs an error. Something has gone wrong.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="exception">Associated exception.</param>
    internal void Error(string message, Exception exception);
}
