using System.Buffers;
using System.Runtime.CompilerServices;
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
        Guard.IsNotNull(array);
        Guard.IsNotNull(random);

        count ??= array.Length;

        if (count <= 1)
        {
            return;
        }

        for (int i = count.Value - 1; i >= 1; i--)
        {
            int j = random.Next(i);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    [MethodImpl(TKConstants.Hot)]
    public static (T[] array, int count) ToRentedArray<T>(this ICollection<T> collection)
    {
        int count = collection.Count;
        T[] array = ArrayPool<T>.Shared.Rent(count);
        collection.CopyTo(array, 0);
        return (array, count);
    }

    public static (T[] array, int count) ToRentedArray<T>(this IEnumerable<T> sequence)
    {
        Guard.IsNotNull(sequence);

        if (sequence is ICollection<T> collection)
        {
            return collection.ToRentedArray();
        }
        else
        {
            int count = 0;
            T[] array = ArrayPool<T>.Shared.Rent(16);
            foreach (var item in sequence)
            {
                if (array.Length == count)
                {
                    T[] backup = array;
                    array = ArrayPool<T>.Shared.Rent(2 * count);
                    backup.CopyTo(array, 0);
                    ArrayPool<T>.Shared.Return(backup);
                }
                array[count] = item;
                count++;
            }

            return (array, count);
        }
    }
}
