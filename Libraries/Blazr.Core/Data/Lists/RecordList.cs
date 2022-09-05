/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class RecordList<TRecord> : IEnumerable<TRecord>
    where TRecord : class, new()
{
    private List<TRecord>? _records = new List<TRecord>();
    private ListState _listState = new ListState();

    public ListState ListState 
        => _listState with { };

    public bool IsPaging 
        => (_listState.PageSize > 0);

    public bool HasList 
        => _records is not null;

    public void Set(ListProviderRequest<TRecord> request, ListProviderResult<TRecord> result)
    {
        _records = result.Items.ToList();
        _listState = _listState with
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            ListTotalCount = result.TotalItemCount,
            SortDescending = this.GetSortDescending(request.SortExpressionString),
            SortField = this.GetSortField(request.SortExpressionString)
        };
    }

    public void Set(IListQuery<TRecord> request, ListProviderResult<TRecord> result)
    {
        _records = result.Items.ToList();
        _listState = _listState with
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            ListTotalCount = result.TotalItemCount,
            SortDescending = this.GetSortDescending(request.SortExpressionString),
            SortField = this.GetSortField(request.SortExpressionString)
        };
    }

    private bool GetSortDescending(string? sortExpressionString)
        => sortExpressionString?.Contains("Desc", StringComparison.CurrentCultureIgnoreCase) ?? false;

    private string? GetSortField(string? sortExpressionString)
        => sortExpressionString?.Replace("Desc", string.Empty, StringComparison.CurrentCultureIgnoreCase).Trim();

    public void Reset()
    {
        _records = null;
        _listState = _listState with { StartIndex = 0, ListTotalCount = 0 };
    }

    public IEnumerator<TRecord> GetEnumerator()
    {
        List<TRecord> list = _records ?? new List<TRecord>();
        foreach (var record in list)
            yield return record;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        List<TRecord> list = _records ?? new List<TRecord>();
        foreach (var record in list)
            yield return record;
    }
}
