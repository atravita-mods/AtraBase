using System.Runtime.CompilerServices;

using AtraBase.Toolkit;

namespace AtraBase.Collections;

/// <summary>
/// Extensions for arrays.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Checks to see if two sequences are equal.
    /// </summary>
    /// <typeparam name="T">Type of array.</typeparam>
    /// <param name="self">First array.</param>
    /// <param name="other">Second array.</param>
    /// <returns>True if their sequences are equal (or if they're both null), false otherwise.</returns>
    [MethodImpl(TKConstants.Hot)]
    public static bool ArraySequenceEquals<T>(this T[]? self, T[]? other)
    {
        if (ReferenceEquals(self, other))
        {
            return true;
        }

        if (self is null)
        {
            return other is null;
        }

        if (other is null)
        {
            return false;
        }

        if (self.Length != other.Length)
        {
            return false;
        }

        for (int i = 0; i < self.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(self[i], other[i]))
            {
                return false;
            }
        }

        return true;
    }
}
