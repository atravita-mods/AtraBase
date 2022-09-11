using System.Text;

namespace AtraBase.Toolkit.Extensions;

/// <summary>
/// Extensions on StringBuilder.
/// </summary>
/// <remarks>Mostly taken from https://github.com/copernicus365/DotNetXtensions/blob/master/DotNetXtensions/src/XStringBuilder.cs 
/// Which is licenced MIT, by https://github.com/copernicus365 .</remarks>
public static class StringBuilderExtensionss
{
    /// <summary>
    /// Trims whitespace from the end of a stringbuilder.
    /// </summary>
    /// <param name="sb">stringbuilder.</param>
    /// <returns>the same stringbuilder, whitespace trimmed.</returns>
    [return: NotNullIfNotNull("sb")]
    public static StringBuilder? TrimEnd(this StringBuilder sb)
    {
        if (sb is null || sb.Length == 0)
        {
            return sb;
        }

        int i = sb.Length;
        while (--i >= 0)
        {
            if (!char.IsWhiteSpace(sb[i]))
            {
                break;
            }
        }

        if (i < sb.Length - 1)
        {
            sb.Length = i + 1;
        }

        return sb;
    }

    /// <summary>
    /// Trims a specific character from the end of a stringbuilder.
    /// </summary>
    /// <param name="sb">stringbuilder.</param>
    /// <param name="ch">character to trim.</param>
    /// <returns>the stringbuilder (trimmed).</returns>
    [return: NotNullIfNotNull("sb")]
    public static StringBuilder? TrimEnd(this StringBuilder sb, char ch)
    {
        if (sb is null || sb.Length == 0)
        {
            return sb;
        }

        int i = sb.Length;
        while (--i >= 0)
        {
            if (sb[i] != ch)
            {
                break;
            }
        }

        if (i < sb.Length - 1)
        {
            sb.Length = i + 1;
        }

        return sb;
    }

    /// <summary>
    /// Trims whitespace from the start of a stringbuilder.
    /// </summary>
    /// <param name="sb"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull("sb")]
    public static StringBuilder? TrimStart(this StringBuilder sb)
    {
        if (sb is null)
        {
            return null;
        }

        int i = 0;
        for (; i < sb.Length; i++)
        {
            if (!char.IsWhiteSpace(sb[i]))
            {
                break;
            }
        }

        if (i == sb.Length)
        {
            sb.Clear();
        }
        else if (i > 0)
        {
            sb.Remove(0, i);
        }
        return sb;
    }

    /// <summary>
    /// Trims a specific character from the start of a stringbuilder.
    /// </summary>
    /// <param name="sb">stringbuilder.</param>
    /// <param name="ch">character to trim.</param>
    /// <returns>the stringbuilder.</returns>
    [return: NotNullIfNotNull("sb")]
    public static StringBuilder? TrimStart(this StringBuilder sb, char ch)
    {
        if (sb is null)
        {
            return null;
        }

        int i = 0;
        for (; i < sb.Length; i++)
        {
            if (ch != sb[i])
            {
                break;
            }
        }

        if (i == sb.Length)
        {
            sb.Clear();
        }
        else if (i > 0)
        {
            sb.Remove(0, i);
        }
        return sb;
    }

    /// <summary>
    /// Trims whitespace from the start and end of a stringbuilder while converting it to a string.
    /// </summary>
    /// <param name="sb">The stringbuilder.</param>
    /// <returns>a string.</returns>
    [return: NotNullIfNotNull("sb")]
    public static string? TrimToString(this StringBuilder sb)
    {
        if (sb is null)
        {
            return null;
        }

        sb.TrimEnd();

        if (sb.Length > 0 && char.IsWhiteSpace(sb[0]))
        {
            for (int i = 0; i < sb.Length; i++)
            {
                if (!char.IsWhiteSpace(sb[i]))
                {
                    return sb.ToString(i);
                }
            }
            return string.Empty;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Trims a specific character from a stringbuilder while converting it to a string.
    /// </summary>
    /// <param name="sb">The stringbuilder.</param>
    /// <param name="ch">Character to trim.</param>
    /// <returns>string, trimmed.</returns>
    [return: NotNullIfNotNull("sb")]
    public static string? TrimToString(this StringBuilder sb, char ch)
    {
        if (sb is null)
        {
            return null;
        }

        sb.TrimEnd();

        if (sb.Length > 0 && char.IsWhiteSpace(sb[0]))
        {
            for (int i = 0; i < sb.Length; i++)
            {
                if (ch != sb[i])
                {
                    return sb.ToString(i);
                }
            }
            return string.Empty;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Turns a stringbuilder into a string, starting at the given index and continuing to the end of the string.
    /// </summary>
    /// <param name="sb">stringbuilder.</param>
    /// <param name="startIndex">Index to start at.</param>
    /// <returns>string (string.Empty if startIndex is past the end of the stringbuilder).</returns>
    [return: NotNullIfNotNull("sb")]
    public static string? ToString(this StringBuilder sb, int startIndex)
    {
        if (sb is null)
        {
            return null;
        }

        int len = sb.Length - startIndex;

        return sb.Length <= 0 ? string.Empty : sb.ToString(startIndex, len);
    }
}
