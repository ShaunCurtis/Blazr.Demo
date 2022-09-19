
using System.Diagnostics.CodeAnalysis;

using System.Reflection;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class ListState<TRecord>
    where TRecord : class, new()
{
    public string? SortField { get; private set; }

    public bool SortDescending { get; private set; }

    public int PageSize { get; private set; } = 1000;

    public int StartIndex { get;  private set; } = 0;

    public int ListTotalCount { get; private set; } = 0;

    public Expression<Func<TRecord, bool>>? FilterExpression { get; private set; }

    public Expression<Func<TRecord, object>>? SortExpression { get; private set; }

    public int Page => StartIndex / PageSize;

    public void SetFromListState(ListState<TRecord> state)
    {
        this.SortField = state.SortField;
        this.SortDescending = state.SortDescending;
        this.PageSize = state.PageSize;
        this.StartIndex = state.StartIndex;
        this.ListTotalCount = state.ListTotalCount;
        this.FilterExpression = state.FilterExpression;
        this.SortExpression = state.SortExpression;
    }

    public void SetSorting(SortRequest request)
    {
        this.SortField = request.SortField;
        this.SortDescending = request.SortDescending;
        if (request.IsSorting && TryBuildSortExpression(request.SortField!, out Expression<Func<TRecord, object>>? expression))
            this.SortExpression = expression;
    }

    public void SetPaging(PagingRequest rquest)
    {
        this.StartIndex = rquest.StartIndex;
        this.PageSize = rquest.PageSize;
    }

    public void SetPaging(int startIndex, int pageSize, Expression<Func<TRecord, bool>>? filterExpression = null)
    {
        this.StartIndex = startIndex;
        this.PageSize = pageSize;
        this.FilterExpression = filterExpression;
    }

    public void SetPaging(int startIndex, Expression<Func<TRecord, bool>>? filterExpression = null)
    {
        this.StartIndex = startIndex;
        this.FilterExpression = filterExpression;
    }

    public void Set(ListProviderRequest<TRecord> request, ListProviderResult<TRecord> result)
    {
        this.PageSize = request.PageSize;
        this.StartIndex = request.StartIndex;
        this.ListTotalCount = result.TotalItemCount;
    }

    public void Set(IListQuery<TRecord> request, ListProviderResult<TRecord> result)
    {
        this.PageSize = request.PageSize;
        this.StartIndex = request.StartIndex;
        this.ListTotalCount = result.TotalItemCount;
    }

    private static bool TryBuildSortExpression(string sortField, [NotNullWhen(true)] out Expression<Func<TRecord, object>>? expression)
    {
        expression = null;

        Type recordType = typeof(TRecord);
        PropertyInfo sortProperty = recordType.GetProperty(sortField)!;
        if (sortProperty is null)
            return false;

        ParameterExpression parameterExpression = Expression.Parameter(recordType, "item");
        MemberExpression memberExpression = Expression.Property((Expression)parameterExpression, sortField);
        Expression propertyExpression = Expression.Convert(memberExpression, typeof(object));

        expression =  Expression.Lambda<Func<TRecord, object>>(propertyExpression, parameterExpression);

        return true;
    }
}
