using System.Linq.Expressions;
using System.Reflection;

namespace AtraBase.Toolkit.Reflection;

/// <summary>
/// Makes delegates from reflection stuff.
/// </summary>
/// <remarks>Inspired by https://github.com/ameisen/SV-SpriteMaster/blob/master/SpriteMaster/Extensions/ReflectionExtDelegates.cs .</remarks>
public static class FastReflection
{
    /// <summary>
    /// Gets a getter for an instance field.
    /// </summary>
    /// <typeparam name="TObject">Effective type of the object.</typeparam>
    /// <typeparam name="TField">Effective type of the field.</typeparam>
    /// <param name="field">The fieldinfo.</param>
    /// <returns>Delegate that gets the field's value.</returns>
    /// <exception cref="ArgumentException">Type mismatch.</exception>
    [return: NotNullIfNotNull("field")]
    public static Func<TObject, TField>? GetInstanceFieldGetter<TObject, TField>(this FieldInfo? field)
    {
        if (field is null)
        {
            return null;
        }
        if (!typeof(TObject).IsAssignableFrom(field.DeclaringType))
        {
            throw new ArgumentException($"{typeof(TObject).FullName} is not assignable from {field.DeclaringType?.FullName}");
        }
        if (!typeof(TField).IsAssignableFrom(field.FieldType))
        {
            throw new ArgumentException($"{typeof(TField).FullName} is not assignable from {field.FieldType.FullName}");
        }
        if (field.IsStatic)
        {
            throw new ArgumentException($"Expected a non-static field");
        }

        ParameterExpression? objparam = Expression.Parameter(typeof(TObject), "obj");
        MemberExpression? fieldgetter = Expression.Field(objparam, field);
        return Expression.Lambda<Func<TObject, TField>>(fieldgetter, objparam).Compile();
    }

    /// <summary>
    /// Gets a setter for an instance field.
    /// </summary>
    /// <typeparam name="TObject">Effective type of the object.</typeparam>
    /// <typeparam name="TField">Effective type of the field.</typeparam>
    /// <param name="field">The fieldinfo.</param>
    /// <returns>Delegate that allows setting the field's value.</returns>
    /// <exception cref="ArgumentException">Type mismatch.</exception>
    [return: NotNullIfNotNull("field")]
    public static Action<TObject, TField>? GetInstanceFieldSetter<TObject, TField>(this FieldInfo? field)
    {
        if (field is null)
        {
            return null;
        }
        if (!typeof(TObject).IsAssignableFrom(field.DeclaringType))
        {
            throw new ArgumentException($"{typeof(TObject).FullName} is not assignable from {field.DeclaringType?.FullName}");
        }
        if (!typeof(TField).IsAssignableTo(field.FieldType))
        {
            throw new ArgumentException($"{typeof(TField).FullName} is not assignable to {field.FieldType.FullName}");
        }
        if (field.IsStatic)
        {
            throw new ArgumentException($"Expected a non-static field");
        }

        ParameterExpression? objparam = Expression.Parameter(typeof(TObject), "obj");
        ParameterExpression? fieldval = Expression.Parameter(typeof(TField), "fieldval");
        UnaryExpression? convertfield = Expression.Convert(fieldval, field.FieldType);
        MemberExpression? fieldsetter = Expression.Field(objparam, field);
        BinaryExpression? assignexpress = Expression.Assign(fieldsetter, convertfield);

        return Expression.Lambda<Action<TObject, TField>>(assignexpress, objparam, fieldval).Compile();
    }

    /// <summary>
    /// Gets a getter for a static field.
    /// </summary>
    /// <typeparam name="TField">Effective type of the field.</typeparam>
    /// <param name="field">Fieldinfo.</param>
    /// <returns>A delegate that allows getting the value from a static field.</returns>
    /// <exception cref="ArgumentException">Type mismatch.</exception>
    [return: NotNullIfNotNull("field")]
    public static Func<TField>? GetStaticFieldGetter<TField>(this FieldInfo? field)
    {
        if (field is null)
        {
            return null;
        }
        if (!typeof(TField).IsAssignableFrom(field.FieldType))
        {
            throw new ArgumentException($"{typeof(TField).FullName} is not assignable from {field.FieldType.FullName}");
        }
        if (!field.IsStatic)
        {
            throw new ArgumentException($"Expected a static field");
        }

        MemberExpression? fieldgetter = Expression.Field(null, field);
        return Expression.Lambda<Func<TField>>(fieldgetter).Compile();
    }

    /// <summary>
    /// Gets a setter for a static field.
    /// </summary>
    /// <typeparam name="TField">Effective type.</typeparam>
    /// <param name="field">Fieldinfo.</param>
    /// <returns>Delegate that allows setting of a static field.</returns>
    /// <exception cref="ArgumentException">Type mismatch.</exception>
    [return: NotNullIfNotNull("field")]
    public static Action<TField>? GetStaticFieldSetter<TField>(this FieldInfo? field)
    {
        if (field is null)
        {
            return null;
        }
        if (!typeof(TField).IsAssignableTo(field.FieldType))
        {
            throw new ArgumentException($"{typeof(TField).FullName} is not assignable to {field.FieldType.FullName}");
        }
        if (!field.IsStatic)
        {
            throw new ArgumentException($"Expected a static field");
        }

        ParameterExpression? fieldval = Expression.Parameter(typeof(TField), "fieldval");
        UnaryExpression? convertfield = Expression.Convert(fieldval, field.FieldType);
        MemberExpression? fieldsetter = Expression.Field(null, field);
        BinaryExpression? assignexpress = Expression.Assign(fieldsetter, convertfield);
        return Expression.Lambda<Action<TField>>(assignexpress, fieldval).Compile();
    }

    [return: NotNullIfNotNull("type")]
    public static Func<object, bool>? GetTypeIs(this Type? type)
    {
        if (type is null)
        {
            return null;
        }
        var obj = Expression.Parameter(typeof(object), "obj");
        var express = Expression.TypeIs(obj, type);
        return Expression.Lambda<Func<object, bool>>(express, obj).Compile();
    }
}