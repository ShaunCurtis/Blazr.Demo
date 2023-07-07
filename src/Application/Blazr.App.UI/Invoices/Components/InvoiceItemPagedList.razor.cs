/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.UI;

public sealed partial class InvoiceItemPagedList
    : PagedListFormBase<InvoiceItem, InvoiceEntityService>, IDisposable
{
    [Inject] private InvoiceAggregateManager Manager { get; set; } = default!;
    [Parameter] public Guid InvoiceUid { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        // Run the base code first
        await base.OnParametersSetAsync();

        if (this.NotInitialized)
        {
            ArgumentNullException.ThrowIfNull(Manager);
            this.NotificationService.RecordChanged += this.OnRecordChanged;
        }

        var filters = new List<FilterDefinition> { new FilterDefinition(ApplicationConstants.InvoiceItem.FilterByInvoiceUid, InvoiceUid.ToString()) };
        this.Presenter.SetFilter(new FilterRequest<InvoiceItem>() { Filters = filters });
    }

    private void OnRecordChanged(object? sender, RecordChangedEventArgs e)
    {
        if (sender != this)
            this.StateHasChanged();
    }

    private async Task OnDeleteAsync(InvoiceItem record)
    {
        if (modalDialog is null)
        {
            this.LogErrorMessage("No modal dialog is configured for confirm dialogs");
            return;
        }

        var options = new BSModalOptions() { ModalSize = BsModalSizes.Normal };
        options.ControlParameters.Add("Message", "Confirm you want to delete this Invoice Item.");

        var result = await modalDialog.ShowAsync<CancelConfirm>(options);

        if (result.ResultType == ModalResultType.OK)
        {
            // Mark the invoiceitem as deleted
            this.Manager.Record.RemoveCollectionItem(record);
        }

        this.NotificationService.NotifyRecordChanged(this, Manager.Record);
        this.StateHasChanged();
    }
    private async Task OnResetAsync()
    {
        if (modalDialog is null)
        {
            this.LogErrorMessage("No modal dialog is configured for confirm dialogs");
            return;
        }

        var options = new BSModalOptions() { ModalSize = BsModalSizes.Normal };
        options.ControlParameters.Add("Message", "Confirm you want to reset the Invoice Item list.");

        var result = await modalDialog.ShowAsync<CancelConfirm>(options);

        if (result.ResultType == ModalResultType.OK)
        {
            this.Manager.Record.ResetCollectionItems();
        }

        this.NotificationService.NotifyRecordChanged(this, Manager.Record);
        this.StateHasChanged();
    }

    private async Task OnNewAsync()
    {
        if (modalDialog is null)
        {
            this.LogErrorMessage("No modal dialog set for editing an invoice item");
            return;
        }
        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", Guid.Empty);

        await modalDialog.ShowAsync<InvoiceItemEditForm>(options);

        this.StateHasChanged();
    }

    protected override async Task OnEditAsync(InvoiceItem record)
    {
        if (modalDialog is null)
        {
            await this.LogErrorMessageAsync("No modal dialog set for editing an invoice item");
            return;
        }

        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", record.Uid);

        await modalDialog.ShowAsync<InvoiceItemEditForm>(options);

        this.StateHasChanged();
    }

    protected override Task OnViewAsync(InvoiceItem record)
        => this.LogErrorMessageAsync("No Viewer is available for invoice items");

    protected override Task OnDashboardAsync(InvoiceItem record)
        => this.LogErrorMessageAsync("No Dashboard is available for invoice items");

    public void Dispose()
        => this.NotificationService.RecordChanged -= this.OnRecordChanged;
}
