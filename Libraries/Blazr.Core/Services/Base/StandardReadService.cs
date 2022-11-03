/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.AspNetCore.Components;

namespace Blazr.Core;

public class StandardReadService<TRecord, TEntity>
    : BaseViewService<TRecord, TEntity>, IReadService<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    public TRecord? Record { get; private set; }

    public StandardReadService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService, NavigationManager navigationManager)
        : base(dataBroker, notifier, authenticationState, authorizationService, navigationManager)
    { }

    public async ValueTask<bool> LoadRecordAsync(Guid Id)
    {
        var result = await this.GetRecordAsync(Id);

        var haveAuthorizedRecord = result.Success
            && result.Record is not null
            && await this.CheckRecordAuthorization(result.Record, this.ReadPolicy);

        this.Record = haveAuthorizedRecord
            ? result.Record
            : new TRecord();

        this.Message = result.Success && !haveAuthorizedRecord
            ? "You are not authorized to access the record"
            : this.Message;

        return haveAuthorizedRecord;
    }
}

