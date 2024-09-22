/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class EditPresenter<TRecord, TIdentity, TEditContext> : IEditPresenter<TRecord, TIdentity, TEditContext>
    where TRecord : class, new()
    where TIdentity : IEntityKey
    where TEditContext : IRecordEditContext<TRecord>, new()
{
    private readonly IDataBroker _dataBroker;
    private readonly IToastService _toastService;
    private readonly INewRecordProvider<TRecord> _newRecordProvider;
    private readonly string _recordName;
    private readonly ILogger<EditPresenter<TRecord, TIdentity, TEditContext>> _logger;

    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public EditContext EditContext { get; private set; }
    public TEditContext RecordEditContext { get; private set; }
    public bool IsNew { get; private set; }

    internal EditPresenter(IDataBroker dataBroker, INewRecordProvider<TRecord> newRecordProvider,
        IToastService toastService, ILogger<EditPresenter<TRecord, TIdentity, TEditContext>> logger)
    {
        _dataBroker = dataBroker;
        _toastService = toastService;
        _newRecordProvider = newRecordProvider;
        _logger = logger;
        this.EditContext = default!;
        this.RecordEditContext = default!;
        _recordName = typeof(TRecord).Name;
    }

    // When called with an Id that doesn't exist the method sets the LastResult
    // to the returned failure result and loads a new entity.
    // It's up to the UI to decide hoe to handle that context
    internal async Task LoadAsync(TIdentity id, bool isNew)
    {
        this.LastDataResult = DataResult.Success();

        TRecord? item = null;

        // The Update Path.  Get the requested record if it exists
        if (!isNew)
        {
            var request = ItemQueryRequest<TIdentity>.Create(id);
            var result = await _dataBroker.ExecuteQueryAsync<TRecord, TIdentity>(request);
            LastDataResult = result;

            if (!this.LastDataResult.Successful)
            {
                var message = result.Message ?? $"The {_recordName} could not be loaded.";
                _logger.LogError(message);
            }

            if (this.LastDataResult.Successful)
            {
                item = result.Item;
                this.IsNew = false;
            }
        }

        // isNew is true or there's no record with the provided Id
        if (item is null)
        {
            item = _newRecordProvider.NewRecord();
            this.IsNew = true;
        }

        this.RecordEditContext = new();
        RecordEditContext.Load(item);
        this.EditContext = new(this.RecordEditContext);
        return;
    }

    public async Task<IDataResult> SaveItemAsync()
    {
        if (!this.RecordEditContext.IsDirty)
        {
            var message = $"The {_recordName} has not changed and therefore has not been updated.";

            _logger.LogWarning(message);
            this.LastDataResult = DataResult.Failure(message);
            _toastService.ShowWarning(message);
            return this.LastDataResult;
        }

        var record = RecordEditContext.AsRecord;
        var command = new CommandRequest<TRecord>(record, this.IsNew ? CommandState.Add : CommandState.Update);
        var result = await _dataBroker.ExecuteCommandAsync(command);

        if (result.Successful)
        {
            var message = $"The {_recordName} was saved.";
            _logger.LogInformation(message);
            _toastService.ShowSuccess(message);
        }
        else
        {
            var message = result.Message ?? $"The {_recordName} could not be saved.";
            _logger.LogError(message);
            _toastService.ShowError(message);
        }

        this.LastDataResult = result;
        return result;
    }
}
