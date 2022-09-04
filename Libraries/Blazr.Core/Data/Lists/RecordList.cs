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

    public ListState ListState => _listState with { };

    public int PageSize => _listState.PageSize;

    public int StartIndex => _listState.StartIndex;

    public int ListTotalCount => _listState.ListTotalCount;

    public bool IsPaging => (PageSize > 0);

    public bool HasList => _records is not null;

    public RecordList()
    { }

    public void Set(ListProviderRequest<TRecord> request, ListProviderResult<TRecord> result)
    {
        _records = result.Items.ToList();
        _listState = _listState with { PageSize = request.PageSize, StartIndex = request.StartIndex, ListTotalCount = result.TotalItemCount };
    }

    public void Set(IListQuery<TRecord> request, ListProviderResult<TRecord> result)
    {
        _records = result.Items.ToList();
        _listState = _listState with { PageSize = request.PageSize, StartIndex = request.StartIndex, ListTotalCount = result.TotalItemCount };
    }

    public void Set(List<TRecord>? records, ListState listState)
    {
        _records = records;
        _listState = listState;
    }

    public void Reset()
    {
        _records = null;
        _listState = _listState with {StartIndex = 0, ListTotalCount = 0 };
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
