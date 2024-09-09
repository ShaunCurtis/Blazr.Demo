/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class DataGridPresenter : IDataGridPresenter
{
    private readonly IDataBroker _dataBroker;
    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public int DefaultPageSize { get; set; } = 20;
    public List<FilterDefinition>? Filters { get; set; }

    internal DataGridPresenter(IDataBroker dataBroker)
    {
        _dataBroker = dataBroker;
    }

    public async ValueTask<GridItemsProviderResult<TGridItem>> GetItemsAsync<TGridItem>(GridItemsProviderRequest<TGridItem> request)
        where TGridItem : class
    {
        // Get the defined sorters
        List<SortDefinition>? sorters = null;

        var definedSorters = request.GetSortByProperties();
        if (definedSorters is not null)
        {
            sorters = new();
            foreach (var sorter in definedSorters)
            {
                var sortDefinition = new SortDefinition()
                {
                    SortField = sorter.PropertyName,
                    SortDescending = sorter.Direction == SortDirection.Descending
                };
                sorters.Add(sortDefinition);
            }
        }

        // Define the Query Request
        var listRequest = new ListQueryRequest()
        {
            StartIndex = request.StartIndex,
            PageSize = request.Count ?? this.DefaultPageSize,
            Sorters = sorters ?? Enumerable.Empty<SortDefinition>(),
            Filters = this.Filters ?? Enumerable.Empty<FilterDefinition>()
        };

        var result = await _dataBroker.ExecuteQueryAsync<TGridItem>(listRequest);
        this.LastDataResult = result;

        return new GridItemsProviderResult<TGridItem>() { Items = result.Items.ToList(), TotalItemCount = result.TotalCount };
    }
}
