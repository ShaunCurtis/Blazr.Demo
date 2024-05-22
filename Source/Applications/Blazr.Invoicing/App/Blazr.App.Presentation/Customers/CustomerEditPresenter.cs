/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class CustomerEditPresenter
{
    private readonly IItemRequestHandler<DmoCustomer> _itemRequestHandler;
    private readonly ICommandHandler<DmoCustomer> _commandHandler;
    private readonly IToastService _toastService;

    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public EditContext EditContext { get; private set; }
    public CustomerEditContext RecordEditContext { get; private set; }
    public bool IsNew { get; private set; }

    public CustomerEditPresenter(IItemRequestHandler<DmoCustomer> itemRequestHandler, ICommandHandler<DmoCustomer> commandHandler, IToastService toastService)
    {
        _itemRequestHandler = itemRequestHandler;
        _commandHandler = commandHandler;
        this.RecordEditContext = new(new());
        this.EditContext = new(this.RecordEditContext);
        _toastService = toastService;
    }

    public async Task LoadAsync(CustomerId id)
    {
        this.LastDataResult = DataResult.Success();
        this.IsNew = false;

        // The Update Path.  Get the requested record if it exists
        if (id.Value != Guid.Empty)
        {
            var request = ItemQueryRequest.Create(id.Value);
            var result = await _itemRequestHandler.ExecuteAsync(request);
            LastDataResult = result;
            if (this.LastDataResult.Successful)
            {
                RecordEditContext = new(result.Item!);
                this.EditContext = new(this.RecordEditContext);
            }
            return;
        }

        // The new path.  Get a new record
        this.RecordEditContext = new(new() { CustomerId = new(Guid.NewGuid()) });
        this.EditContext = new(this.RecordEditContext);
        this.IsNew = true;
    }

    public async Task<IDataResult> SaveItemAsync()
    {

        if (!this.RecordEditContext.IsDirty)
        {
            this.LastDataResult = DataResult.Failure("The record has not changed and therefore has not been updated.");
            _toastService.ShowWarning("The record has not changed and therefore has not been updated.");
            return this.LastDataResult;
        }

        var record = RecordEditContext.AsRecord;
        var command = new CommandRequest<DmoCustomer>(record, this.IsNew ? CommandState.Add : CommandState.Update);
        var result = await _commandHandler.ExecuteAsync(command);

        if (result.Successful)
            _toastService.ShowSuccess("The Customer was saved.");
        else
            _toastService.ShowError(result.Message ?? "The Customer could not be saved.");

        this.LastDataResult = result;
        return result;
    }
}
