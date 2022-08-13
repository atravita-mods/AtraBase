using CommunityToolkit.Diagnostics;

namespace AtraBase.Toolkit.Extensions;

public static class ArrayExtensions
{
    /// <summary>
    /// Fisher-Yates shuffle, assuming only the first <paramref name="count" /> elements are
    /// relevant.
    /// </summary>
    /// <typeparam name="T">Type of array.</typeparam>
    /// <param name="array"></param>
    /// <param name="random"></param>
    /// <param name="count"></param>
    /// <remarks>This is for use in array pools.</remarks>
    public static void Shuffle<T>(this T[] array, Random random, int? count = null)
    {
        Guard.IsNotNull(array, nameof(array));
        Guard.IsNotNull(random, nameof(random));

        count ??= array.Length;

        if (count <= 1)
        {
            return;
        }

        for (int i = count.Value - 1; i >= 1 ; i--)
        {
            int j = random.Next(i);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
