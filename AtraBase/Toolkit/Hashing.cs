﻿using CommunityToolkit.Diagnostics;

namespace AtraBase.Toolkit;

public static class Hashing
{
    // taken from https://stackoverflow.com/a/36845864/19366602 by Scott Chamberlain,
    // who originally derived it from dotnet code (which is licensed MIT to the NET Foundation).
    // todo: this is probably a good candidate for vectorization.
    // anyways will use this for now.
    public static int GetStableHashCode(this string str)
    {
        Guard.IsNotNull(str);
        unchecked
        {
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i + 1] == '\0')
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}
