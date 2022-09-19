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
    private ListState<TRecord> _listState = new ListState<TRecord>();

    public ListState<TRecord> ListState 
        => _listState;

    public bool IsPaging 
        => (_listState.PageSize > 0);

    public bool HasList 
        => _records is not null;

    public void Set(ListProviderRequest<TRecord> request, ListProviderResult<TRecord> result)
    {
        _records = result.Items.ToList();
        _listState.Set(request, result);
    }

    public void Set(IListQuery<TRecord> request, ListProviderResult<TRecord> result)
    {
        _records = result.Items.ToList();
        _listState.Set(request, result);
    }

    public void Reset()
    {
        _records = null;
        _listState.SetPaging(0);
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
