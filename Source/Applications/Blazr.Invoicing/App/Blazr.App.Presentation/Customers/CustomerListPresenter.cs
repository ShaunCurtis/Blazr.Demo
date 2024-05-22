/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class CustomerListPresenter
{
    private readonly IListRequestHandler<DmoCustomer> _listRequestHandler;
    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public int DefaultPageSize { get; set; } = 20;

    public CustomerListPresenter(IListRequestHandler<DmoCustomer> listRequestHandler)
    {
        _listRequestHandler = listRequestHandler;
    }

    public async ValueTask<GridItemsProviderResult<DmoCustomer>> GetItemsAsync<TGridItem>(GridItemsProviderRequest<DmoCustomer> request)
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
            Sorters = sorters ?? Enumerable.Empty<SortDefinition>() 
        };

        var result = await _listRequestHandler.ExecuteAsync(listRequest);
        this.LastDataResult = result;

        return new GridItemsProviderResult<DmoCustomer>() { Items = result.Items.ToList(), TotalItemCount = result.TotalCount };
    }
}
