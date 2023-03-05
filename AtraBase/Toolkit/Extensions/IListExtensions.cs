using System.Runtime.CompilerServices;

using CommunityToolkit.Diagnostics;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions on IList.
/// </summary>
public static class IListExtensions
{
    /// <summary>
    /// Gets the Nth occurrence of a specific thing a list.
    /// </summary>
    /// <typeparam name="T">Type of the list.</typeparam>
    /// <param name="list">List to search in.</param>
    /// <param name="item">Item to search for.</param>
    /// <param name="count">N.</param>
    /// <returns>Index of the thing, or -1 if not found.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int NthOccuranceOf<T>(this IList<T> list, T item, int count = 1)
        where T : IEquatable<T>
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(item) && --count <= 0)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Clears the nulls from a list.
    /// </summary>
    /// <typeparam name="T">Type of list.</typeparam>
    /// <param name="list">List to clear nulls from.</param>
    /// <remarks>Clears in-place.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void ClearNulls<T>(this IList<T>? list)
    {
        if (list?.Count is 0 or null)
        {
            return;
        }

        // clear all nulls from the end first.
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] is null)
            {
                list.RemoveAt(i);
            }
            else
            {
                break;
            }
        }

        int count = list.Count;
        for (int i = 0; i < count - 1; i++)
        {
            if (list[i] is null)
            {
                // swap with last element.
                (list[i], list[count - 1]) = (list[count - 1], list[i]);

                // remove last element.
                list.RemoveAt(count - 1);

                // reduce.
                count--;
            }
        }

        if (list[^1] is null)
        {
            list.RemoveAt(list.Count - 1);
        }
    }

    /// <summary>
    /// Removes the item at <paramref name="index"/> by swapping it with the last element.
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    /// <param name="list">The list to remove from.</param>
    /// <param name="index">The index to remove.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void SwapRemoveAt<T>(this IList<T>? list, int index)
    {
        if (list is null)
        {
            return;
        }
        Guard.IsBetween(index, -1, list.Count);

        if (index != list.Count - 1)
        {
            (list[index], list[^1]) = (list[^1], list[index]);
        }
        list.RemoveAt(list.Count - 1);
    }
}