// Ignore Spelling: evt Utils

using System.Diagnostics;
using System.Runtime.CompilerServices;

using AtraBase.Internal;
using AtraBase.Toolkit.Extensions;

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
    /// <param name="evt">Event to raise.</param>
    /// <param name="sender">Sending instance, or null for none.</param>
    /// <param name="name">Name (for logging).</param>
    public static void RaiseSafe(this EventHandler? evt, object? sender, [CallerArgumentExpression("evt")] string name = "")
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(name), "Name should not be null or whitespace");

        Delegate[]? handlers = evt?.GetInvocationList();

        if (handlers?.Length is null or 0)
        {
            return;
        }

        Guard.IsNotNullOrWhiteSpace(name);
        Logger.Instance.Verbose($"Raising from {name}");

        foreach (EventHandler? handler in handlers.FilterToType<Delegate, EventHandler>())
        {
            try
            {
                handler?.Invoke(sender, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Exception while handling event {name}:", ex);
            }
        }
    }

    /// <summary>
    /// Raises an event safely, logging errors if they appear. See also: <seealso cref="EventHandler"/>.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of the event args.</typeparam>
    /// <param name="evt">Event to raise.</param>
    /// <param name="sender">Sender of the event, or null for none.</param>
    /// <param name="args">Event arguments.</param>
    /// <param name="name">Name (for logging).</param>
    public static void RaiseSafe<TEventArgs>(this EventHandler<TEventArgs>? evt, object? sender, TEventArgs args, [CallerArgumentExpression("evt")] string name = "")
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(name), "Name should not be null or whitespace");

        Delegate[]? handlers = evt?.GetInvocationList();

        if (handlers?.Length is null or 0)
        {
            return;
        }

        Guard.IsNotNullOrWhiteSpace(name);
        Logger.Instance.Verbose($"Raising from {name}");

        foreach (EventHandler<TEventArgs>? handler in handlers.FilterToType<Delegate, EventHandler<TEventArgs>>())
        {
            try
            {
                handler?.Invoke(sender, args);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Exception while handling event {name}:", ex);
            }
        }
    }
}