using System.Runtime.CompilerServices;

namespace AtraBase.Toolkit;

/// <summary>
/// Utilities to help deal with types.
/// </summary>
public static class TypeUtils
{
    /// <summary>
    /// Checks if an object is exactly the runtime type of <typeparamref name="Tderived"/>.
    /// </summary>
    /// <typeparam name="Tparent">Original type of object.</typeparam>
    /// <typeparam name="Tderived">Type to check against.</typeparam>
    /// <param name="obj">Instance to check.</param>
    /// <param name="derived">The object, cast to the derived type.</param>
    /// <returns>True if the type exactly matches, false otherwise.</returns>
    [MethodImpl(TKConstants.Hot)]
    public static bool IsExactlyOfType<Tparent, Tderived>([NotNullWhen(true)] Tparent obj, [NotNullWhen(true)] out Tderived? derived)
        where Tderived : Tparent
    {
        if (obj?.GetType() == typeof(Tderived))
        {
            derived = (Tderived)obj;
            return true;
        }
        derived = default;
        return false;
    }
}