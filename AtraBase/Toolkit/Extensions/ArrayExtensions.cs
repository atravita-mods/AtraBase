using System.Buffers;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions for use for arrays.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Skips a certain number of items from an array.
    /// </summary>
    /// <typeparam name="T">Type of the array.</typeparam>
    /// <param name="array">Array.</param>
    /// <param name="count">Number of items to skip.</param>
    /// <returns>An array segment with the remaining items.</returns>
    public static ArraySegment<T> SkipToSegment<T>(this T[] array, int count)
    {
        Guard.IsNotNull(array);
        Guard.IsGreaterThanOrEqualTo(count, 0);

        if (count >= array.Length)
        {
            return ArraySegment<T>.Empty;
        }

        return new(array, count, array.Length - count);
    }

    /// <summary>
    /// Fisher-Yates shuffle, assuming only the first <paramref name="count" /> elements are
    /// relevant.
    /// </summary>
    /// <typeparam name="T">Type of array.</typeparam>
    /// <param name="array">The array to shuffle.</param>
    /// <param name="random">The random to use.</param>
    /// <param name="count">The number of relevant elements.</param>
    /// <remarks>This is for use in array pools.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
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

    /// <summary>
    /// Copies a collection to an rented array.
    /// </summary>
    /// <typeparam name="T">Type param.</typeparam>
    /// <param name="collection">The collection to copy.</param>
    /// <returns>A rented array and the number of relevant items.</returns>
    [MethodImpl(TKConstants.Hot)]
    public static (T[] array, int count) ToRentedArray<T>(this ICollection<T> collection)
    {
        int count = collection.Count;
        T[] array = ArrayPool<T>.Shared.Rent(count);
        collection.CopyTo(array, 0);
        return (array, count);
    }

    /// <summary>
    /// Copies a collection to an rented array.
    /// </summary>
    /// <typeparam name="T">Type param.</typeparam>
    /// <param name="sequence">The collection to copy.</param>
    /// <returns>A rented array and the number of relevant items.</returns>
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
            foreach (T? item in sequence)
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
