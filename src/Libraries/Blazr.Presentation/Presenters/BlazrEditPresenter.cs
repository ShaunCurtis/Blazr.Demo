/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation;

public class BlazrEditPresenter<TRecord, TEntityService, TEditContext>
    : IBlazrEditPresenter<TRecord, TEditContext>
    where TRecord : class, IStateEntity, IEntity, new()
    where TEditContext : class, IBlazrRecordEditContext<TRecord>, new()
    where TEntityService : class, IEntityService
{
    private readonly IDataBroker _dataBroker;
    private readonly INotificationService<TEntityService> _notificationService;
    private readonly ILogger<BlazrEditPresenter<TRecord, TEntityService, TEditContext>> _logger;

    public IDataResult LastResult { get; protected set; } = CommandResult.Success();

    public TEditContext RecordContext { get; protected set; } = new();

    public EditContext EditContext { get; protected set; } = default!;

    public BlazrEditPresenter(IDataBroker dataBroker, INotificationService<TEntityService> notificationService, ILogger<BlazrEditPresenter<TRecord, TEntityService, TEditContext>> logger)
    {
        _dataBroker = dataBroker;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async ValueTask LoadAsync(EntityUid id)
        => await GetItemAsync(new ItemQueryRequest { Uid = id });

    public ValueTask ResetItemAsync()
    {
        EditContext.ResetEditState();
        this.EditContext.NotifyFieldChanged(new FieldIdentifier(RecordContext, "Uid"));
        return ValueTask.CompletedTask;
    }

    public ValueTask SaveItemAsync()
        => this.UpdateAsync();

    public async ValueTask DeleteItemAsync()
    {
        RecordContext.SetAsDeleted();
        await this.UpdateAsync();
    }

    protected virtual async ValueTask GetItemAsync(ItemQueryRequest request)
    {
        this.RecordContext.

        if (!request.Uid.IsEmpty)
        {
            ItemQueryResult<TRecord> result = await _dataBroker.GetItemAsync<TRecord>(request);

            if (result.Successful && result.Item is not null)
                RecordContext.Load(result.Item);
        }


        this.EditContext = new EditContext(RecordContext);
    }

    protected virtual async ValueTask UpdateAsync()
    {
        LastResult = CommandResult.Failure("Nothing to Do");

        if (!this.RecordContext.IsDirty)
            return;

        var record = this.RecordContext.AsRecord;

        if (record.Uid.IsEmpty)
        {
            LastResult = CommandResult.Failure("No commands can be run on an empty Uid.");
            return;
        }

        var request = new CommandRequest<TRecord> { Item = record };

        LastResult = await _dataBroker.ExecuteCommandAsync<TRecord>(request);

        if (LastResult.Successful)
            EditContext?.SetEditStateAsSaved();

        this.LogResult();

        this.Notify();
    }

    protected void Notify()
        => _notificationService.NotifyRecordChanged(this, this.RecordContext.AsRecord);


    protected void LogResult(IDataResult? result = null)
    {
        result ??= this.LastResult;
        if (!result.Successful)
            _logger.LogError(result.Message);

        this.LastResult = result;
    }
}