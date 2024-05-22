/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

//TODO - I don't think we use this any more
public class InvoiceCompositePresenter
{
    private readonly IDataBroker _dataBroker;
    private readonly IToastService _toastService;

    public IDataResult LastDataResult { get; private set; } = DataResult.Success();

    public InvoiceComposite Composite { get; private set; }

    public IQueryable<DmoInvoiceItem> InvoiceItems => this.Composite.InvoiceItems.AsQueryable();

    public InvoiceCompositePresenter(IToastService toastService, IDataBroker dataBroker)
    {
        _toastService = toastService;
        _dataBroker = dataBroker;
        this.Composite = new InvoiceComposite(new(), Enumerable.Empty<DmoInvoiceItem>(), true);
    }

    public async Task LoadAsync(InvoiceId id)
    {
        this.LastDataResult = DataResult.Success();

        if (id.Value != Guid.Empty)
        {
            var request = ItemQueryRequest.Create(id.Value);
            var result = await _dataBroker.ExecuteQueryAsync<InvoiceComposite>(request);
            LastDataResult = result;
            if (this.LastDataResult.Successful)
            {
                this.Composite = result.Item!;
            }
            return;
        }
    }

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
