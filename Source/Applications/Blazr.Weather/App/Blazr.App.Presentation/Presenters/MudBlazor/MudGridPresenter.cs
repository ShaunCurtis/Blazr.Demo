/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using MudBlazor;

namespace Blazr.App.Presentation.Mud;

public class MudGridPresenter<TRecord> : IMudGridListPresenter<TRecord>
    where TRecord : class, new()
{
    private readonly IDataBroker _dataBroker;
    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public int DefaultPageSize { get; set; } = 20;
    public List<FilterDefinition>? Filters { get; set; }

    public MudGridPresenter(IDataBroker dataBroker)
    {
        _dataBroker = dataBroker;
    }

    public async Task<GridData<TRecord>> GetItemsAsync(GridState<TRecord> request)
    {
        // Get the defined sorters
        List<SortDefinition>? sorters = null;

        foreach (var sorter in request.SortDefinitions)
        {
            var sortDefinition = new SortDefinition()
            {
                SortField = sorter.SortBy,
                SortDescending = sorter.Descending
            };

            if (sorters is null)
                sorters = new();

            sorters.Add(sortDefinition);
        }

        // Define the Query Request
        var listRequest = new ListQueryRequest()
        {
            StartIndex = request.Page * request.PageSize,
            PageSize = request.PageSize,
            Sorters = sorters ?? Enumerable.Empty<SortDefinition>(),
            Filters = this.Filters ?? Enumerable.Empty<FilterDefinition>()
        };

        var result = await _dataBroker.ExecuteQueryAsync<TRecord>(listRequest);
        this.LastDataResult = result;

        return new GridData<TRecord>() { Items = result.Items.ToList(), TotalItems = result.TotalCount };
    }
}
