/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class InvoiceCompositePresenter
{
    private readonly IDataBroker _dataBroker;
    private readonly IToastService _toastService;
    private readonly INewRecordProvider<DmoInvoice> _recordProvider;

    public IDataResult LastDataResult { get; private set; } = DataResult.Success();

    public InvoiceComposite Composite { get; private set; }

    public IQueryable<DmoInvoiceItem> InvoiceItems => this.Composite.InvoiceItems.AsQueryable();

    public InvoiceCompositePresenter(IToastService toastService, IDataBroker dataBroker, INewRecordProvider<DmoInvoice> recordProvider)
    {
        _toastService = toastService;
        _dataBroker = dataBroker;
        _recordProvider = recordProvider;

        // Build a new context
        this.Composite = new InvoiceComposite(_recordProvider.NewRecord(), Enumerable.Empty<DmoInvoiceItem>(), true);
    }

    public async Task LoadAsync(InvoiceId id)
    {
        this.LastDataResult = DataResult.Success();

        // if we have an empty guid them we go with the new context created in the constructor
        if (id.Value != Guid.Empty)
        {
            var request = ItemQueryRequest<InvoiceId>.Create(id);
            var result = await _dataBroker.ExecuteQueryAsync<InvoiceComposite, InvoiceId>(request);
            LastDataResult = result;
            if (this.LastDataResult.Successful)
            {
                this.Composite = result.Item!;
            }
            return;
        }
    }

    //TODO - I don;t think this is needed anymore
    //public async Task<IDataResult> SaveItemAsync()
    //{

    //    if (!this.RecordEditContext.IsDirty)
    //    {
    //        this.LastDataResult = DataResult.Failure("The record has not changed and therefore has not been updated.");
    //        _toastService.ShowWarning("The record has not changed and therefore has not been updated.");
    //        return this.LastDataResult;
    //    }

    //    var record = RecordEditContext.AsRecord;
    //    var command = new CommandRequest<DmoCustomer>(record, this.IsNew ? CommandState.Add : CommandState.Update);
    //    var result = await _commandHandler.ExecuteAsync(command);

    //    if (result.Successful)
    //        _toastService.ShowSuccess("The Customer was saved.");
    //    else
    //        _toastService.ShowError(result.Message ?? "The Customer could not be saved.");

    //    this.LastDataResult = result;
    //    return result;
    //}
}
