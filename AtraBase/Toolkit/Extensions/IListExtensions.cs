namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions on IList.
/// </summary>
public static class IListExtensions
{
    /// <summary>
    /// Gets the Nth occurance of a specific thing a list.
    /// </summary>
    /// <typeparam name="T">Type of the list.</typeparam>
    /// <param name="list">List to search in.</param>
    /// <param name="item">Item to search for.</param>
    /// <param name="count">N.</param>
    /// <returns>Index of the thing, or -1 if not found.</returns>
    [Pure]
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
    public static void ClearNulls<T>(this IList<T> list)
    {
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

                // remove last element
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
}