/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Blazr.App.Presentation;

/// <summary>
/// Custom List Presenter that overrides the getItemsAsync methods to get the dataset 
/// from the InvoiceAggregateManager rather than the databroker 
/// </summary>
public class InvoiceItemListPresenter
    : ListPresenter<InvoiceItem, InvoiceEntityService>
{
    private readonly InvoiceAggregateManager _aggregateManager;
    public InvoiceItemListPresenter(IDataBroker dataBroker, IUiStateService uiStateService, IListController<InvoiceItem> controller, INotificationService<InvoiceEntityService> notificationService, InvoiceAggregateManager aggregateManager)
        : base(dataBroker, uiStateService, controller, notificationService)
    {
        _aggregateManager = aggregateManager;
    }

    protected override async ValueTask getItemsAsync(ListQueryRequest request, object? sender = null)
    {

        var result = await _aggregateManager.GetItemsFromAggregateAsync(request);

        // Check if the requested page is beyond the count
        // We may have filtered down to a much small list
        // If so reset the request and requery to get the first page
        if (result.Successful && request.StartIndex >= result.TotalCount)
        {
            request = request with { StartIndex = 0 };
            result = await _aggregateManager.GetItemsFromAggregateAsync(request);
        }

        if (result.Successful)
        {
            _listController.Set(request, result);
            _listController.NotifyStateChanged(sender);
        }

        _listController.NotifyStateChanged(sender);
    }

    protected override async ValueTask<ItemsProviderResult<InvoiceItem>> getItemsAsync(ItemsProviderRequest itemsRequest)
    {
        var request = new ListQueryRequest() { StartIndex = itemsRequest.StartIndex, PageSize = itemsRequest.Count };

        var result = await _aggregateManager.GetItemsFromAggregateAsync(request);

        return result.Successful
            ? new ItemsProviderResult<InvoiceItem>(result.Items, result.TotalCount > int.MaxValue ? int.MaxValue : (int)result.TotalCount)
            : new ItemsProviderResult<InvoiceItem>(Enumerable.Empty<InvoiceItem>(), 0);
    }



}
