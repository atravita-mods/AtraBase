﻿namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions for Random.
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Prewarms a random.
    /// </summary>
    /// <param name="random">The random to prewarm.</param>
    /// <returns>The random.</returns>
    public static Random PreWarm(this Random random)
    {
        int prewarm = random.Next(64);
        for (int i = 0; i < prewarm; i++)
        {
            random.NextDouble();
        }

        prewarm = random.Next(64);
        for (int i = 0; i < prewarm; i++)
        {
            random.NextDouble();
        }

        return random;
    }
}
