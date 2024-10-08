﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class InvoiceItemEditPresenter
{
    private readonly IToastService _toastService;
    private readonly InvoiceComposite _composite;
    private InvoiceItemId _invoiceItemId = InvoiceItemId.NewEntity;

    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public EditContext EditContext { get; private set; }
    public DmoInvoiceItemEditContext RecordEditContext { get; private set; }
    public bool IsNew { get; private set; }

    internal InvoiceItemEditPresenter(InvoiceComposite composite, IToastService toastService, InvoiceItemId id)
    {
        _composite = composite;
        _toastService = toastService;

        // Detect if we have a new item request.
        this.IsNew = id == InvoiceItemId.NewEntity;
        
        var item = this.Load(id);

        RecordEditContext = new(item);
        this.EditContext = new(this.RecordEditContext);
        _invoiceItemId = this.RecordEditContext.Id;
    }

    private DmoInvoiceItem Load(InvoiceItemId id)
    {
        this.LastDataResult = DataResult.Success();

        var item = IsNew
            ? _composite.GetNewInvoiceItem()
            : _composite.GetInvoiceItem(id).Item;

        if (item is null)
        {
            this.LastDataResult = DataResult.Failure("The record does not exist.");
            _toastService.ShowError("The record does not exist.");
            return new();
        }

        return item;
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
            var success = _composite.DispatchInvoiceItemAction(this.RecordEditContext.Id, new AddInvoiceItemAction(this, this.RecordEditContext.AsRecord));

            if (success.Successful)
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

        this.LastDataResult = _composite.DispatchInvoiceItemAction(this.RecordEditContext.Id, new UpdateInvoiceItemAction(this, this.RecordEditContext.AsRecord));

        if (this.LastDataResult.Successful)
            _toastService.ShowSuccess("The invoice item was updated.");
        else
            _toastService.ShowError(this.LastDataResult.Message ?? "The Invoice Item could not be added to the invoice.");

        return Task.FromResult(this.LastDataResult);
    }

    public Task<IDataResult> DeleteItemAsync()
    {
        if (IsNew)
        {
            var message = "You cn't delete an item that you haven't created.";
            _toastService.ShowError(message);
            this.LastDataResult = DataResult.Failure(message);

            return Task.FromResult(this.LastDataResult);
        }

        this.LastDataResult = _composite.DispatchInvoiceItemAction(this.RecordEditContext.Id, new DeleteInvoiceItemAction(this));

        if (this.LastDataResult.Successful)
            _toastService.ShowSuccess("The invoice item was deleted.");
        else
            _toastService.ShowError(this.LastDataResult.Message ?? "The Invoice Item could not be deleted from the invoice.");

        return Task.FromResult(this.LastDataResult);
    }

    public static InvoiceItemEditPresenter CreateInstance(InvoiceComposite composite, IToastService toastService, InvoiceItemId id)
    {
        var presenter = new InvoiceItemEditPresenter(composite, toastService, id);

        return presenter;
    }
}
