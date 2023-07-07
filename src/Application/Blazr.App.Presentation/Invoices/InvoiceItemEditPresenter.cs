/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Presentation;

/// <summary>
/// Custom Edit Presenter that overrides the getItemAsync and UpdateAsync methods 
/// to get from and set to the InvoiceAggregateManager rather than the databroker 
/// </summary>
public sealed class InvoiceItemEditPresenter : BlazrEditPresenter<InvoiceItem, InvoiceEntityService, InvoiceItemEditContext>
{
    private InvoiceAggregateManager _aggregateManager;

    public InvoiceItemEditPresenter(IDataBroker dataBroker, INotificationService<InvoiceEntityService> notificationService, ILogger<InvoiceItemEditPresenter> logger, InvoiceAggregateManager aggregateManager)
        : base(dataBroker, notificationService, logger)
            => _aggregateManager = aggregateManager;

    protected override ValueTask GetItemAsync(ItemQueryRequest request)
    {
        // Creates a new invoice item
        if (request.Uid == Guid.Empty)
        {
            var newRecord = InvoiceFactory.New(_aggregateManager.Record.Root);
            RecordContext.Load(newRecord);
            this.EditContext = new EditContext(RecordContext);
            LastResult = ItemQueryResult<InvoiceItem>.Success(newRecord);
            return ValueTask.CompletedTask;
        }

        // Get the record from the Aggregate
        var result = _aggregateManager.Record.GetCollectionItem(request);
        if (result.Successful && result.Item is not null)
        {
            RecordContext.Load(result.Item);
            this.EditContext = new EditContext(RecordContext);
        }
        LastResult = result;
        return ValueTask.CompletedTask;
    }

    protected override ValueTask UpdateAsync()
    {
        LastResult = CommandResult.Success();
        _aggregateManager.Record.SaveCollectionItem(this.RecordContext.AsRecord);

        EditContext.SetEditStateAsSaved();
        this.LogResult();
        this.Notify();

        return ValueTask.CompletedTask;
    }
}
