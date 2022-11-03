
using Microsoft.AspNetCore.Components;
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
    public RecordList<TRecord> Records { get; private set; } = new RecordList<TRecord>();

    public StandardListService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService, NavigationManager navigationManager)
        : base(dataBroker, notifier, authenticationState, authorizationService, navigationManager)
    { }

    public async ValueTask<bool> GetRecordsAsync(ListProviderRequest<TRecord> request)
        => await this.GetRecordsAsync(ListQuery<TRecord>.GetQuery(request));

    public async ValueTask<bool> GetRecordsAsync(ListQuery<TRecord> query)
    {
        this.Message = String.Empty;

        var result = await this.DataBroker.ExecuteAsync<TRecord>(query);

        if (result.Success)
        {
            this.Records.Set(query, result);
            return true;
        }

        this.Records.Reset();
        this.Message = $"Failed to retrieve the recordset at index {query.StartIndex}";
        return false;
    }
}

