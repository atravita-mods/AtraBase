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
}