/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class InvoiceEditPresenter
{
    private readonly IToastService _toastService;
    private readonly InvoiceComposite _composite;

    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public EditContext EditContext { get; private set; }
    public DmoInvoiceEditContext RecordEditContext { get; private set; }
    public bool IsNew => _composite.State == DiodeState.New;

    public InvoiceEditPresenter(InvoiceComposite composite, IToastService toastService)
    {
        _composite = composite;
        _toastService = toastService;
        this.RecordEditContext = new(composite.Invoice);
        this.EditContext = new(this.RecordEditContext);
    }

    public Task<IDataResult> SaveItemAsync()
    {

        if (!this.RecordEditContext.IsDirty)
        {
            this.LastDataResult = DataResult.Failure("The record has not changed and therefore has not been updated.");
            _toastService.ShowWarning("The record has not changed and therefore has not been updated.");
            return Task.FromResult(this.LastDataResult);
        }

        this.LastDataResult = _composite.UpdateInvoice(this.RecordEditContext.Mutate).ToDataResult();

        if (this.LastDataResult.Successful)
            _toastService.ShowSuccess("The invoice data was updated.");
        else
            _toastService.ShowError(this.LastDataResult.Message ?? "The invoice data could not be updated.");

        return Task.FromResult(this.LastDataResult);
    }
}
