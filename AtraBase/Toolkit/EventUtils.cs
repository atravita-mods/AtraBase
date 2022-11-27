using System.Runtime.CompilerServices;

using AtraBase.Internal;

using CommunityToolkit.Diagnostics;

namespace AtraBase.Toolkit;

/// <summary>
/// Helps in raising events safely.
/// </summary>
/// <remarks>Inspired by casey here: https://github.com/spacechase0/StardewValleyMods/blob/develop/SpaceShared/Util.cs 
/// who originally took the code from SMAPI.</remarks>
public static class EventUtils
{
    /// <summary>
    /// Raises an event that has no event args safely, logging errors if they appear. See also: <seealso cref="EventHandler"/>.
    /// </summary>
    /// <param name="handlers">Listeners.</param>
    /// <param name="sender">Sending instance, or null for none.</param>
    /// <param name="name">Name (for logging).</param>
    public static void RaiseSafe(this Delegate[]? handlers, object? sender, [CallerMemberName] string name = "")
    {
        if (handlers?.Length is null or 0)
        {
            return;
        }

        Guard.IsNotNullOrWhiteSpace(name);
        Logger.Instance.Verbose($"Raising from {name}");

        EventArgs? args = new();
        foreach (EventHandler handler in handlers.OfType<EventHandler>())
        {
            try
            {
                handler.Invoke(sender, args);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Exception while handling event {name}:\n\n{ex}");
            }
        }
    }

    /// <summary>
    /// Raises an event safely, logging errors if they appear. See also: <seealso cref="EventHandler"/>.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of the event args.</typeparam>
    /// <param name="handlers">Listeners.</param>
    /// <param name="sender">Sender of the event, or null for none.</param>
    /// <param name="args">Event arguments.</param>
    /// <param name="name">Name (for logging).</param>
    public static void RaiseSafe<TEventArgs>(this Delegate[]? handlers, object? sender, TEventArgs args, [CallerMemberName] string name = "")
    {
        if (handlers?.Length is null or 0)
        {
            return;
        }

        Guard.IsNotNullOrWhiteSpace(name);
        Logger.Instance.Verbose($"Raising from {name}");

        foreach (EventHandler<TEventArgs> handler in handlers.OfType<EventHandler<TEventArgs>>())
        {
            try
            {
                handler.Invoke(sender, args);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Exception while handling event {name}:\n\n{ex}");
            }
        }
    }
}