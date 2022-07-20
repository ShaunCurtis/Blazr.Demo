/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class StandardReadService<TRecord, TEntity>
    : BaseViewService<TRecord, TEntity>, IReadService<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected string ReadPolicy = "IsViewerPolicy";

    public TRecord? Record { get; private set; }

    public bool HasRecord => this.Record is not null;

    public StandardReadService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService)
        : base(dataBroker, notifier, authenticationState, authorizationService)
    { }

    public async ValueTask<bool> GetRecordAsync(Guid Id)
    {
        this.Message = String.Empty;
        this.Record = null;

        var result = await this.DataBroker.ExecuteAsync<TRecord>(new RecordGuidKeyQuery<TRecord>(Id));

        if (result.Success && result.Record is not null)
        {
            if (!await this.CheckRecordAuthorization(result.Record, this.ReadPolicy))
            {
                this.Record = new TRecord();
                return false;
            }

            this.Record = result.Record;

        }
        else
            this.Message = $"Failed to retrieve the record with Id - {Id}";

        return result.Success;
    }
}

