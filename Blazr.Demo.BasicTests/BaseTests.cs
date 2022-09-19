using Blazr.App.Core;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace Blazr.Demo.BasicTests;

public class BaseTests
{
    [Fact]
    public void Test1()
    {
        //Expression<Func<DvoWeatherForecast, object>> exp = (DvoWeatherForecast item) => item.TemperatureC;
        var exp = GetExpression<DvoWeatherForecast>("TemperatureC")!;
        var x = exp.Compile();

    }

    private Expression<Func<TRecord, object>>? GetExpression<TRecord>(string sortField)
    {
        Type recordType = typeof(TRecord);
        ParameterExpression parameter = Expression.Parameter(recordType, "item");
        
        PropertyInfo sortProperty = recordType.GetProperty(sortField)!;

        if (sortProperty is null)
            return null;

        Expression expressionToUse = (Expression)parameter;

        MemberExpression memberExpression = Expression.Property(expressionToUse, sortField);

        Expression propertyExpression = Expression.Convert(memberExpression, typeof(object));

        Expression<Func<TRecord, object>> complexExpression = Expression.Lambda<Func<TRecord, object>>(propertyExpression, parameter);

        return complexExpression;
    }
}