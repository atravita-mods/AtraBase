﻿namespace AtraBase.Collections;

/// <summary>
/// Extensions for enumerables.
/// </summary>
public static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] enumerables)
        => enumerables.SelectMany(a => a);

    public static IEnumerable<T> Flatten<T>(IEnumerable<IEnumerable<T>> enumerables)
        => enumerables.SelectMany(a => a);
}