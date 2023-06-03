using System.Runtime.CompilerServices;

using CommunityToolkit.Diagnostics;

namespace AtraBase.Toolkit.Extensions;

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

    /// <summary>
    /// Returns true with a probability of <paramref name="chance"/>.
    /// </summary>
    /// <param name="random">Random to use.</param>
    /// <param name="chance">Probability.</param>
    /// <returns>true with the probability of <paramref name="chance"/>, false otherwise.</returns>
    [MethodImpl(TKConstants.Hot)]
    public static bool OfChance(this Random random, double chance)
    {
        if (chance <= 0)
        {
            return false;
        }
        else if (chance >= 1)
        {
            return true;
        }
        else
        {
            return random.NextDouble() < chance;
        }
    }

    /// <summary>
    /// Returns true with a probability of 1/<paramref name="sides"/>. Basically, the probability of rolling any particular side on an N sided dice.
    /// </summary>
    /// <param name="random">Random to use.</param>
    /// <param name="sides">In effect, the number of sides on the dice.</param>
    /// <returns>True with the probability of 1/<paramref name="sides"/>, false otherwise.</returns>
    [MethodImpl(TKConstants.Hot)]
    public static bool RollDice(this Random random, int sides)
    {
        Guard.IsGreaterThanOrEqualTo(sides, 1);

        if (sides == 1)
        {
            return true;
        }
        else
        {
            return random.Next(sides) == 0;
        }
    }
}
