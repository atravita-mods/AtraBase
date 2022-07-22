namespace AtraBase.Events;

/// <summary>
/// An event manager that uses weak references. Order is not guarenteed.
/// </summary>
/// <typeparam name="TEventArgs">The type for the event arguments.</typeparam>
public interface IWeakEventManager<TEventArgs>
{
    /// <summary>
    /// Adds an listener.
    /// </summary>
    /// <param name="listener">listener to add.</param>
    void Add(EventHandler<TEventArgs> listener);

    /// <summary>
    /// Removes all listeners matching this listener.
    /// </summary>
    /// <param name="listener">listener to remove.</param>
    void Remove(EventHandler<TEventArgs> listener);

    /// <summary>
    /// Raises the event.
    /// </summary>
    /// <param name="sender">sender.</param>
    /// <param name="args">Event args.</param>
    void Raise(object? sender, TEventArgs args);
}
