/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class StandardListService<TRecord, TEntity>
    : BaseViewService<TRecord, TEntity>, IListService<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    public int PageSize { get; protected set; }

    public int StartIndex { get; protected set; }

    public int ListCount { get; protected set; }

    public IEnumerable<TRecord>? Records { get; private set; }

    public bool IsPaging => (PageSize > 0);

    public bool HasList => this.Records is not null;

    public StandardListService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService)
        : base(dataBroker, notifier, authenticationState, authorizationService)
    { }

    public async ValueTask<ItemsProviderResult<TRecord>> GetRecordsAsync(ItemsProviderRequest itemsRequest)
    {
        await this.GetRecordsAsync(new ListProviderRequest<TRecord>(itemsRequest));
        return new ItemsProviderResult<TRecord>(this.Records ?? new List<TRecord>(), this.ListCount);
    }

    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(ListProviderRequest<TRecord> request)
        => await this.GetRecordsAsync(new ListQuery<TRecord>(request));

    public async ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(IListQuery<TRecord> query)
    {
        this.Message = String.Empty;
        this.PageSize = query.PageSize;
        this.StartIndex = query.StartIndex;

        var result = await this.DataBroker.ExecuteAsync<TRecord>(query);

        return this.GetResult(result);
    }

    private ListProviderResult<TRecord> GetResult(ListProviderResult<TRecord> result)
    {

        if (result.Success && result.Items is not null)
        {
            this.Records = result.Items;
            this.ListCount = result.TotalItemCount;
            var page = this.StartIndex <= 0
                ? 0
                : (int)(this.StartIndex / this.PageSize);

            this.Notifier.NotifyListPaged(this, page);
        }
        else
        {
            this.Records = null;
            this.ListCount = 0;
            this.Message = $"Failed to retrieve the recordset at index {this.StartIndex}";
        }

        return new ListProviderResult<TRecord>(this.Records ?? Enumerable.Empty<TRecord>(), this.ListCount, result.Success, Message);
    }
}

