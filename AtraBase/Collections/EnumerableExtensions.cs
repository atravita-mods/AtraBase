// Ignore Spelling: Concat

namespace AtraBase.Collections;

/// <summary>
/// Extensions for enumerables.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Concatenates multiple enumerables together.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerables">The enumerables.</param>
    /// <returns></returns>
    public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] enumerables)
        => enumerables.SelectMany(a => a);

    public static IEnumerable<T> Flatten<T>(IEnumerable<IEnumerable<T>> enumerables)
        => enumerables.SelectMany(a => a);
}