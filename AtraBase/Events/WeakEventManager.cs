using AtraBase.Internal;

namespace AtraBase.Events;

#warning - cross check this for thread safety.

/// <inheritdoc />
internal class WeakEventManager<TEventArgs> : IWeakEventManager<TEventArgs>
{
    private readonly List<WeakReference<EventHandler<TEventArgs>>> listeners = new();
    private readonly List<WeakReference<EventHandler<TEventArgs>>> toAdd = new();
    private readonly HashSet<EventHandler<TEventArgs>> toRemove = new();

    /// <inheritdoc />
    public void Add(EventHandler<TEventArgs> listener)
    {
        lock (this.toAdd)
        {
            this.toAdd.Add(new WeakReference<EventHandler<TEventArgs>>(listener));
        }
    }

    /// <inheritdoc />
    /// <remarks>We don't actually remove now, since that may cause issues
    /// whenever a listener tries to remove themselves during an event.
    /// These are weak events anyways, so GC can still come collecting.</remarks>
    public void Remove(EventHandler<TEventArgs> listener)
    {
        lock (this.toRemove)
        {
            this.toRemove.Add(listener);
        }
    }

    /// <inheritdoc />
    public void Raise(object? sender, TEventArgs args)
    {
        lock (this.toAdd)
        {
            // add the listeners registered since last time.
            this.listeners.AddRange(this.toAdd);
            this.toAdd.Clear();
        }

        for (int i = this.listeners.Count - 1; i >= 0; i--)
        {
            if (!this.listeners[i].TryGetTarget(out EventHandler<TEventArgs>? listener))
            {
                RemoveListenerAt(i);
                continue;
            }

            lock (this.toRemove)
            {
                if (this.toRemove.Contains(listener))
                {
                    RemoveListenerAt(i);
                    this.toRemove.Remove(listener);
                    continue;
                }
            }

            try
            {
                listener.Invoke(sender, args);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Failed while raising weak event:\n\n{ex}");
            }
        }

        void RemoveListenerAt(int i)
        {
            if (i != this.listeners.Count - 1)
            {
                (this.listeners[i], this.listeners[^1]) = (this.listeners[^1], this.listeners[i]);
            }
            this.listeners.RemoveAt(this.listeners.Count - 1);
        }
    }
}
