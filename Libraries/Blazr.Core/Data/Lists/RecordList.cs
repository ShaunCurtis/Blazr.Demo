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
    public int PageSize { get; private set; }

    public int StartIndex { get; private set; }

    public int ListCount { get; private set; }

    public bool IsPaging => (PageSize > 0);

    public bool HasList => _records is not null;

    public RecordList(List<TRecord>? records, int pageSize, int startIndex, int listCount)
    {
        _records = records;
        PageSize = pageSize;
        StartIndex = startIndex;
        ListCount = listCount;
    }
    public RecordList(ListProviderRequest<TRecord> request, ListProviderResult<TRecord> result)
    {
        _records = result.Items.ToList();
        PageSize = request.PageSize;
        StartIndex = request.StartIndex;
        ListCount = result.TotalItemCount;
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
