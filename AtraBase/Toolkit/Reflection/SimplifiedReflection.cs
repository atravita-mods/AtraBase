using System.Reflection;
using AtraBase.Internal;

using CommunityToolkit.Diagnostics;
using HarmonyLib;

namespace AtraBase.Toolkit.Reflection;

/// <summary>
/// Holds methods to simplify getting methods via reflection.
/// This class doesn't throw errors if the method is not found.
/// And only searches the class given, it doesn't search up the hierarchy.
/// </summary>
public static class SimplifiedReflection
{
    private const BindingFlags UnflattenedInstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private const BindingFlags UnflattenedStaticFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

    /// <summary>
    /// Gets the named instance method if declared on this class.
    /// Returns null if not found.
    /// </summary>
    /// <param name="type">Type to get the instance method from.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>Instance method's methodinfo.</returns>
    public static MethodInfo? DeclaredInstanceMethodNamedOrNull(this Type type, string methodName)
        => type.GetMethod(methodName, UnflattenedInstanceFlags);

    /// <summary>
    /// Gets the named instance method if declared on this class (with a specific call pattern).
    /// Returns null if not found.
    /// </summary>
    /// <param name="type">Type to get the instance method from.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="paramsList">Call pattern of the method.</param>
    /// <returns>Instance method's methodinfo.</returns>
    public static MethodInfo? DeclaredInstanceMethodNamedOrNull(this Type type, string methodName, Type[] paramsList)
        => type.GetMethod(methodName, UnflattenedInstanceFlags, null, paramsList, null);

    /// <summary>
    /// Gets the named static method if declared on this class.
    /// Returns null if not found.
    /// </summary>
    /// <param name="type">Type to get the static method from.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>Static method's methodinfo.</returns>
    /// <exception cref="MethodNotFoundException">The static method was not found.</exception>
    public static MethodInfo? DeclaredStaticMethodNamedOrNull(this Type type, string methodName)
        => type.GetMethod(methodName, UnflattenedStaticFlags);

    /// <summary>
    /// Gets the named static method (with a specific call pattern) if declared on this class.
    /// Returns null if not found.
    /// </summary>
    /// <param name="type">Type to get the static method from.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="paramsList">Call pattern of the method.</param>
    /// <returns>Static method's methodinfo.</returns>
    /// <exception cref="MethodNotFoundException">The static method was not found.</exception>
    public static MethodInfo? DeclaredStaticMethodNamedOrNull(this Type type, string methodName, Type[] paramsList)
        => type.GetMethod(methodName, UnflattenedStaticFlags, null, paramsList, null);

    /// <summary>
    /// Searches through all assemblies, getting any types that can be assigned to the indicated type.
    /// This gets subclasses (and will pick up on the original type).
    /// Skips dynamic assemblies though.
    /// </summary>
    /// <param name="type">Type to find subclasses of.</param>
    /// <param name="publiconly">Whether to search public classes only.</param>
    /// <param name="includeAbstract">Whether or not to include abstract classes and interfaces.</param>
    /// <param name="assemblyfilter">A function to filter the assemblies to search.</param>
    /// <param name="typefilter">A function to filter the types to search.</param>
    /// <returns>A list of types.</returns>
    /// <remarks>This is quite slow, use with caution.</remarks>
    public static HashSet<Type> GetAssignableTypes(
        this Type type,
        bool publiconly = false,
        bool includeAbstract = false,
        Func<Assembly, bool>? assemblyfilter = null,
        Func<Assembly, Type, bool>? typefilter = null)
    {
        HashSet<Type> types = new();
        assemblyfilter ??= (Assembly assembly) => true;
        typefilter ??= (Assembly assembly, Type type) => true;
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assemblyfilter))
        {
            if (assembly.IsDynamic)
            {
                continue;
            }
            if (publiconly)
            {
                try
                {
                    types.UnionWith(assembly.GetExportedTypes().Where((Type t) => (includeAbstract || !t.IsAbstract) && t.IsAssignableTo(type) && typefilter(assembly, type)));
                }
                catch (Exception ex) when (
                    ex is NotSupportedException
                    or FileNotFoundException
                    or TypeLoadException)
                {
                    Logger.Instance.Warn($"Searching for types in {assembly.FullName} seems to have failed.\n\n{ex}");
                }
            }
            else
            {
                try
                {
                    types.UnionWith(assembly.GetTypes().Where((Type t) => (includeAbstract || !t.IsAbstract) && t.IsAssignableTo(type) && typefilter(assembly, type)));
                }
                catch (Exception ex) when (
                    ex is ReflectionTypeLoadException
                    or TypeLoadException)
                {
                    Logger.Instance.Warn($"Searching for types in {assembly.FullName} seems to have failed.\n\n{ex}");
                }
            }
        }
        return types;
    }

    /// <summary>
    /// Gets the direct base method of a method, or null if not applicable.
    /// </summary>
    /// <param name="method">Method to check.</param>
    /// <returns>MethodBase of the base method, or null if not applicable.</returns>
    public static MethodBase? GetBaseMethod(this MethodBase method)
    {
        Guard.IsNotNull(method);
        Guard.IsFalse(method.IsStatic);

        Type? baseType = method.DeclaringType?.BaseType;
        if (baseType is null)
        {
            return null;
        }

        if (!method.IsVirtual)
        {
            return null;
        }

        Type[] parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        flags |= method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
        switch (method)
        {
            case ConstructorInfo constructorInfo:
                return baseType.GetConstructor(flags, null, parameters, null);
            case MethodInfo methodInfo:
                return baseType.GetMethod(method.Name, flags, null, parameters, null);
            default:
                ThrowHelper.ThrowInvalidOperationException($"Expected {method.FullDescription()} to be either a ConstructorInfo or a MethodInfo");
                return null;
        }
    }
}