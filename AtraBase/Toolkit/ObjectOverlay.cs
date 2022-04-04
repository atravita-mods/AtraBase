using System.Linq.Expressions;
using System.Reflection;
using AtraBase.Toolkit.Reflection;

namespace AtraBase.Toolkit;

internal static class ObjectOverlay
{
    internal static Action<T, T> CreateObjOverlayFunction<T>()
    {
        ParameterExpression? obj = Expression.Parameter(typeof(T));
        ParameterExpression? overlay = Expression.Parameter(typeof(T));

        List<Expression> expressionList = new();

        foreach (PropertyInfo? property in typeof(T).GetProperties())
        {
            List<Expression> miniList = new();
            MemberExpression? getter = Expression.Property(overlay, property);
            MemberExpression? setter = Expression.Property(obj, property);

            ParameterExpression? overrideValue = Expression.Parameter(property.PropertyType, "value");

            BinaryExpression? assign = Expression.Assign(overrideValue, getter);
            miniList.Add(assign);
            if (property.PropertyType.IsValueType)
            {
                BinaryExpression? assignValueType = Expression.Assign(setter, overrideValue);
                miniList.Add(assignValueType);
            }
            else
            {
                ConditionalExpression? nullcheck = Expression.IfThen(
                    Expression.Call(typeof(ObjectOverlay).StaticMethodNamed(nameof(ObjectOverlay.IsNotNull)), overrideValue),
                    Expression.Assign(setter, overrideValue));
                miniList.Add(nullcheck);
            }
            BlockExpression? blockexp = Expression.Block(
                new ParameterExpression[] { overrideValue },
                miniList);
            expressionList.Add(blockexp);
        }

        BlockExpression? block = Expression.Block(expressionList);
        return Expression.Lambda<Action<T, T>>(block, new ParameterExpression[] { obj, overlay } ).Compile();
    }

    private static bool IsNotNull(object obj)
    {
        return obj is not null;
    }
}