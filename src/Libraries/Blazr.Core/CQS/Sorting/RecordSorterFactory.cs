/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Reflection;

namespace Blazr.Core;

public static class RecordSorterFactory
{
    public static bool TryBuildSortExpression<TRecord>(string sortField, [NotNullWhen(true)] out Expression<Func<TRecord, object>>? expression)
        where TRecord: class
    {
        expression = null;

        Type recordType = typeof(TRecord);
        PropertyInfo sortProperty = recordType.GetProperty(sortField)!;
        if (sortProperty is null)
            return false;

        ParameterExpression parameterExpression = Expression.Parameter(recordType, "item");
        MemberExpression memberExpression = Expression.Property((Expression)parameterExpression, sortField);
        Expression propertyExpression = Expression.Convert(memberExpression, typeof(object));

        expression = Expression.Lambda<Func<TRecord, object>>(propertyExpression, parameterExpression);

        return true;
    }
}

