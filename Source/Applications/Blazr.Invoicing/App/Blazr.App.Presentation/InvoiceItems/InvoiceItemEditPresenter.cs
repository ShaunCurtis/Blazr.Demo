/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class InvoiceItemEditPresenter
{
    private readonly IToastService _toastService;
    private readonly InvoiceComposite _composite;
    private InvoiceItemId _invoiceItemId = new(Guid.Empty);

    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public EditContext EditContext { get; private set; }
    public DmoInvoiceItemEditContext RecordEditContext { get; private set; }
    public bool IsNew { get; private set; }

    public InvoiceItemEditPresenter(InvoiceComposite composite, IToastService toastService)
    {
        _composite = composite;
        _toastService = toastService;
        this.RecordEditContext = new(new());
        this.EditContext = new(this.RecordEditContext);
    }

    public Task LoadAsync(InvoiceItemId id)
    {
        this.LastDataResult = DataResult.Success();
        this.IsNew = false;

        var item = _composite.GetInvoiceItem(id);

        if (id.Value == Guid.Empty)
        {
            item = _composite.GetNewInvoiceItem();
            this.IsNew = true;
        }

        if (item is null)
        {
            this.LastDataResult = DataResult.Failure("The record does not exist.");
            _toastService.ShowError("The record does not exist.");
            return Task.CompletedTask;
        }

        RecordEditContext = new(item);
        this.EditContext = new(this.RecordEditContext);
        _invoiceItemId = this.RecordEditContext.Id;

        return Task.CompletedTask;
    }

    public Task<IDataResult> SaveItemAsync()
    {

        if (!this.RecordEditContext.IsDirty)
        {
            this.LastDataResult = DataResult.Failure("The record has not changed and therefore has not been updated.");
            _toastService.ShowWarning("The record has not changed and therefore has not been updated.");
            return Task.FromResult(this.LastDataResult);
        }

        if (IsNew)
        {
            var success = _composite.AddInvoiceItem(this.RecordEditContext.AsRecord);

            if (success)
            {
                var message = "The Invoice Item was added to the invoice.";
                _toastService.ShowSuccess(message);
                this.LastDataResult = DataResult.Success(message);
            }
            else
            {
                var message = "The Invoice Item could not be added to the invoice.";
                _toastService.ShowError(message);
                this.LastDataResult = DataResult.Failure(message);
            }

            return Task.FromResult(this.LastDataResult);
        }

        this.LastDataResult = _composite.UpdateInvoiceItem(_invoiceItemId, this.RecordEditContext.Mutate);

        if (this.LastDataResult.Successful)
            _toastService.ShowSuccess("The invoice item was added/updated.");
        else
            _toastService.ShowError(this.LastDataResult.Message ?? "The Invoice Item could not be added to the invoice.");

        return Task.FromResult(this.LastDataResult);

    }
}
