/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class InvoiceAggregateManager
{
    private readonly IDataBroker _dataBroker;
    private readonly ILogger<InvoiceAggregateManager> _logger;
    private readonly INotificationService<InvoiceEntityService> _notificationService;

    public InvoiceAggregate Record { get; private set; } = new();

    public InvoiceAggregateManager(IDataBroker dataBroker, ILogger<InvoiceAggregateManager> logger, INotificationService<InvoiceEntityService> notificationService)
    {
        _dataBroker = dataBroker;
        _logger = logger;
        _notificationService = notificationService;
    }

    public IDataResult LastResult { get; private set; } = CommandResult.Success();

    public async ValueTask LoadAsync(Guid uid)
    {
        var result = await _dataBroker.GetItemAsync<InvoiceAggregate>(new ItemQueryRequest(uid));

        this.LogResult(result);

        if (result.Successful)
            this.Record = result.Item ?? new();
    }

    public Task ResetAggregateAsync()
    {
        this.Record.ResetAggregate();
        _notificationService.NotifyRecordChanged(this, Record);
        return Task.CompletedTask; ;
    }

    public ValueTask<ListQueryResult<InvoiceItem>> GetItemsFromAggregateAsync(ListQueryRequest request)
    {
        var items = this.Record.LiveItems
            .Skip(request.StartIndex)
            .Take(request.PageSize);

        var count = this.Record.LiveItems.Count();

        return ValueTask.FromResult(ListQueryResult<InvoiceItem>.Success(items, count ));
    }

    public async ValueTask UpdateToDataStoreAsync()
    {
        this.LastResult = await _dataBroker.ExecuteCommandAsync<InvoiceAggregate>(new CommandRequest<InvoiceAggregate>(this.Record));

        this.LogResult();

        if (this.LastResult.Successful)
            this.Record.SetAggregateAsSaved();
    }

    private void LogResult(IDataResult? result = null)
    {
        result ??= this.LastResult;
        if (!result.Successful)
            _logger.LogError(result.Message);

        this.LastResult = result;
    }
}
