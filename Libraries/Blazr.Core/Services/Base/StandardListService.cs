/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class StandardListService<TRecord, TService>
    : IListService<TRecord>
    where TRecord : class, new()
    where TService : class, IEntityService
{
    protected ICQSDataBroker DataBroker;
    protected INotificationService<TService> Notifier;

    public int PageSize { get; protected set; }

    public int StartIndex { get; protected set; }

    public int ListCount { get; protected set; }

    public IEnumerable<TRecord>? Records { get; private set; }

    public TRecord? Record { get; private set; }

    public string? Message { get; protected set; }

    public bool IsPaging => (PageSize > 0);

    public bool HasList => this.Records is not null;

    public bool HasRecord => this.Record is not null;

    public StandardListService(ICQSDataBroker dataBroker, INotificationService<TService> notifier)
    {
        this.DataBroker = dataBroker;
        Notifier = notifier;
    }

    public async ValueTask GetRecordAsync(Guid Id)
    {
        this.Message = String.Empty;
        var result = await this.DataBroker.ExecuteAsync<TRecord>(new RecordGuidKeyQuery<TRecord>(Id));

        if (result.Success && result.Record is not null)
        {
            this.Record = result.Record;
            return;
        }
        this.Record = null;
        this.Message = $"Failed to retrieve the record with Id - {Id.ToString()}";
    }

    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(ListProviderRequest request)
    {
        this.Message = String.Empty;
        this.PageSize = request.PageSize;
        this.StartIndex = request.StartIndex;

        var result = await this.DataBroker.ExecuteAsync<TRecord>( new RecordListQuery<TRecord>(request));

        if (result.Success && result.Items is not null)
        {
            this.Records = result.Items;
            this.ListCount = result.TotalItemCount;
            var page = request.StartIndex <= 0
                ? 0
                : (int)(request.StartIndex / request.PageSize);

            this.Notifier.NotifyListPaged(this, page);
        }
        else
        {
            this.Records = null;
            this.ListCount = 0;
            this.Message = $"Failed to retrieve the recordset at index {request.StartIndex}";
        }

        return new ListProviderResult<TRecord>(this.Records ?? Enumerable.Empty<TRecord>(), this.ListCount, result.Success, Message);
    }

    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(int startRecord, int pageSize)
    {
        this.StartIndex = startRecord;
        this.PageSize = pageSize;

        return await this.GetRecordsAsync();
    }

    public async ValueTask<ItemsProviderResult<TRecord>> GetRecordsAsync(ItemsProviderRequest itemsRequest)
    {
        var request = new ListProviderRequest(itemsRequest);
        this.StartIndex = request.StartIndex;
        this.PageSize = this.PageSize;

        await this.GetRecordsAsync(request);

        return new ItemsProviderResult<TRecord>(this.Records ?? new List<TRecord>(), this.ListCount);
    }

    private async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync()
    {
        var cancel = new CancellationToken();
        var request = new ListProviderRequest(this.StartIndex, this.PageSize, cancel);

        return await this.GetRecordsAsync(request);
    }
}

