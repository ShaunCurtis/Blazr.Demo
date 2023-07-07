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
public sealed class InvoiceEditPresenter : BlazrEditPresenter<Invoice, InvoiceEntityService, InvoiceEditContext>
{
    private InvoiceAggregateManager _aggregateManager;

    public InvoiceEditPresenter(IDataBroker dataBroker, INotificationService<InvoiceEntityService> notificationService, ILogger<InvoiceEditPresenter> logger, InvoiceAggregateManager aggregateManager)
        : base(dataBroker, notificationService, logger)
            => _aggregateManager = aggregateManager;

    protected override ValueTask GetItemAsync(ItemQueryRequest request)
    {
        LastResult = ItemQueryResult<Invoice>.Success(_aggregateManager.Record.Root);
        RecordContext.Load(_aggregateManager.Record.Root);
        this.EditContext = new EditContext(RecordContext);

        return ValueTask.CompletedTask;
    }

    protected override ValueTask UpdateAsync()
    {
        LastResult = CommandResult.Success();
        _aggregateManager.Record.UpdateRoot(this.RecordContext.AsRecord);

        EditContext.SetEditStateAsSaved();
        this.LogResult();
        this.Notify();

        return ValueTask.CompletedTask;
    }
}
