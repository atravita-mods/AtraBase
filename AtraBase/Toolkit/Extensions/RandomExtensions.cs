namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions for Random.
/// </summary>
public static class RandomExtensions
{
    // just a buffer to stash prewarm values in.
    private static byte[] buffer = new byte[8];

    /// <summary>
    /// Prewarms a random.
    /// </summary>
    /// <param name="random">The random to prewarm.</param>
    public static void PreWarm(this Random random)
    {
        int prewarm = random.Next(64);
        for (int i = 0; i < prewarm; i++)
        {
            random.NextBytes(buffer);
        }

        prewarm = random.Next(64);
        for (int i = 0; i < prewarm; i++)
        {
            random.NextBytes(buffer);
        }
    }
}
