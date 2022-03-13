namespace AtraBase.Toolkit.Extensions;

internal static class IListExtensions
{
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