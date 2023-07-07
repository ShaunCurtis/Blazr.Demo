/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public class RecordSorter<TRecord> : IRecordSorter<TRecord>
    where TRecord : class
{
    protected virtual Expression<Func<TRecord, object>>? DefaultSorter => null;
    protected virtual bool DefaultSortDescending { get; }

    public IQueryable<TRecord> AddSortToQuery(IQueryable<TRecord> query, IEnumerable<SortDefinition> definitions)
    {
        if (definitions.Count() == 0)
        {
            query = AddDefaultSort(query);
            return query;
        }

        foreach (var defintion in definitions)
            query = AddSort(query, defintion);

        return query;
    }

    protected IQueryable<TRecord> AddSort(IQueryable<TRecord> query, SortDefinition definition)
    {
        Expression<Func<TRecord, object>>? expression = null;

        if (RecordSorterFactory.TryBuildSortExpression(definition.SortField, out expression))
        {
            if (expression is not null)
            {
                query = definition.SortDescending
                    ? query.OrderByDescending(expression)
                    : query.OrderBy(expression);
            }
        }

        return query;
    }

    protected IQueryable<TRecord> AddDefaultSort(IQueryable<TRecord> query)
    {
        if (this.DefaultSorter is not null)
        {
            query = this.DefaultSortDescending
            ? query.OrderByDescending(this.DefaultSorter)
            : query.OrderBy(this.DefaultSorter);
        }

        return query;
    }
}

