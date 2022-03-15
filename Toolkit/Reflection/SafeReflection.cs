using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AtraBase.Toolkit.Reflection;

/// <summary>
/// Reflection, but throws an error if something's not found.
/// </summary>
/// <remarks>Inspired by https://gitlab.com/theLion/smapi-mods/-/blob/main/Common/Extensions/SafeReflections.cs. </remarks>
public static class SafeReflection
{
    private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
    private const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

    /// <summary>
    /// Gets the default (zero parameter) constructor of a type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <returns>Constructor.</returns>
    /// <exception cref="MethodNotFoundException">Constructor not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstructorInfo Constructor(this Type type)
        => type.GetConstructor(InstanceFlags, null, Array.Empty<Type>(), null)
            ?? throw new MethodNotFoundException(type.FullName + "::" + ".ctor");

    /// <summary>
    /// Get the constructor of a type. Throws an exception of the constructor is not found.
    /// </summary>
    /// <param name="type">Type to get the constructor of.</param>
    /// <param name="paramsList">Call pattern of constructor. .</param>
    /// <returns>Constructor.</returns>
    /// <exception cref="MethodNotFoundException">Constructor was not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstructorInfo Constructor(this Type type, Type[] paramsList)
        => type.GetConstructor(InstanceFlags, null, paramsList, null)
            ?? throw new MethodNotFoundException(type.FullName + "::" + ".ctor");

    /// <summary>
    /// Gets the named instance method.
    /// </summary>
    /// <param name="type">Type to get the instance method from.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>Instance method's methodinfo.</returns>
    /// <exception cref="MethodNotFoundException">The instance method was not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodInfo InstanceMethodNamed(this Type type, string methodName)
        => type.GetMethod(methodName, InstanceFlags)
            ?? throw new MethodNotFoundException(type.FullName + "::" + methodName);

    /// <summary>
    /// Gets the named instance method.
    /// </summary>
    /// <param name="type">Type to get the instance method from.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="paramsList">Call pattern of the method.</param>
    /// <returns>Instance method's methodinfo.</returns>
    /// <exception cref="MethodNotFoundException">The instance method was not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodInfo InstanceMethodNamed(this Type type, string methodName, Type[] paramsList)
        => type.GetMethod(methodName, InstanceFlags, null, paramsList, null)
            ?? throw new MethodNotFoundException(type.FullName + "::" + methodName);

    /// <summary>
    /// Gets the named static method.
    /// </summary>
    /// <param name="type">Type to get the static method from.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>Static method's methodinfo.</returns>
    /// <exception cref="MethodNotFoundException">The static method was not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodInfo StaticMethodNamed(this Type type, string methodName)
        => type.GetMethod(methodName, StaticFlags)
            ?? throw new MethodNotFoundException(type.FullName + "::" + methodName);

    /// <summary>
    /// Gets the named static method (with a specific call pattern).
    /// </summary>
    /// <param name="type">Type to get the static method from.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="paramsList">Call pattern of the method.</param>
    /// <returns>Static method's methodinfo.</returns>
    /// <exception cref="MethodNotFoundException">The static method was not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodInfo StaticMethodNamed(this Type type, string methodName, Type[] paramsList)
        => type.GetMethod(methodName, StaticFlags, null, paramsList, null)
            ?? throw new MethodNotFoundException(type.FullName + "::" + methodName);

    /// <summary>
    /// Gets the instance property such named.
    /// </summary>
    /// <param name="type">Type to search.</param>
    /// <param name="propertyName">Name of property.</param>
    /// <returns>Instance property.</returns>
    /// <exception cref="MethodNotFoundException">Instance property not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyInfo InstancePropertyNamed(this Type type, string propertyName)
        => type.GetProperty(propertyName, InstanceFlags)
            ?? throw new MethodNotFoundException(type.FullName + "::" + propertyName);

    /// <summary>
    /// Gets the static property such named.
    /// </summary>
    /// <param name="type">Type to search.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Property.</returns>
    /// <exception cref="MethodNotFoundException">Property not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyInfo StaticPropertyNamed(this Type type, string propertyName)
        => type.GetProperty(propertyName, StaticFlags)
            ?? throw new MethodNotFoundException(type.FullName + "::" + propertyName);

    /// <summary>
    /// Gets the instance field such named.
    /// </summary>
    /// <param name="type">Type to search.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>Field.</returns>
    /// <exception cref="MethodNotFoundException">Field not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldInfo InstanceFieldNamed(this Type type, string fieldName)
        => type.GetField(fieldName, InstanceFlags)
            ?? throw new MethodNotFoundException(type.FullName + "::" + fieldName);

    /// <summary>
    /// Gets the static field such named.
    /// </summary>
    /// <param name="type">Type to search.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>Field.</returns>
    /// <exception cref="MethodNotFoundException">Field not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldInfo StaticFieldNamed(this Type type, string fieldName)
        => type.GetField(fieldName, StaticFlags)
            ?? throw new MethodNotFoundException(type.FullName + "::" + fieldName);
}