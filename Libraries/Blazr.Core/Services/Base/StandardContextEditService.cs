/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public class StandardEditContextService<TRecord, TEditContext, TEntity>
    : BaseViewService<TRecord, TEntity>, 
        IContextEditService<TEditContext>
    where TRecord : class, new()
    where TEditContext : class, IRecordEditContext<TRecord>, new()
    where TEntity : class, IEntity
{
    public TEditContext EditModel { get; private set; } = new TEditContext();

    public StandardEditContextService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService)
        : base(dataBroker, notifier, authenticationState, authorizationService)
    { }

    public async ValueTask<bool> LoadRecordAsync(Guid Id)
    {
        if (Id == Guid.Empty)
        {
            this.EditModel.Load(new TRecord());
            return true;
        }

        var result = await this.GetRecordAsync(Id);

        var haveAuthorizedRecord = result.Success
            && result.Record is not null
            && await this.CheckRecordAuthorization(result.Record, this.ReadPolicy);

        this.Message = result.Success && !haveAuthorizedRecord
            ? "You are not authorized to access the record"
            : this.Message;

        var record = haveAuthorizedRecord
            ? result.Record!
            : new TRecord();

        this.EditModel.Load(record);

        return haveAuthorizedRecord;
    }

    public async ValueTask<bool> AddRecordAsync()
    {
        this.Message = string.Empty;

        if (!await this.CheckAuthorization(this.AddPolicy))
        {
            this.Message = "You are not authorized to add a record";
            return false;
        }

        if (!EditModel.IsNew)
        {
            this.Message = "You can't add an existing record";
            return false;
        }

        var record = EditModel.AsNewRecord;
        var command = AddRecordCommand<TRecord>.GetCommand(record);
        var result = await this.DataBroker.ExecuteAsync<TRecord>(command);

        if (result.Success)
        {
            this.EditModel.Load(record);
            this.NotifyChange(EditModel.Uid);
        }

        this.Message = result.Success
            ? String.Empty
            : "Failed to add the record";

        return result.Success;
    }

    public async ValueTask<bool> UpdateRecordAsync()
    {
        var record = EditModel.Record;

        if (!await this.CheckRecordAuthorization(this.EditModel.CleanRecord, this.EditPolicy))
            return false;

        var command = UpdateRecordCommand<TRecord>.GetCommand(record);
        var result = await this.DataBroker.ExecuteAsync<TRecord>(command);

        if (result.Success)
        {
            this.EditModel.Load(record);
            this.NotifyChange(EditModel.Uid);
        }

        this.Message = result.Success
            ? String.Empty
            : "Failed to update the record";

        return result.Success;
    }

    public async ValueTask<bool> DeleteRecordAsync()
    {
        var record = EditModel.CleanRecord;
        var id = EditModel.Uid;

        if (id == Guid.Empty)
        {
            this.Message = "No record to delete";
            return false;
        }

        if (!await this.CheckRecordAuthorization(record, this.DeletePolicy))
            return false;

        var command = DeleteRecordCommand<TRecord>.GetCommand(record);
        var result = await this.DataBroker.ExecuteAsync<TRecord>(command);

        if (result.Success)
        {
            this.EditModel.Load(new TRecord());
            this.NotifyChange(id);
        }

        this.Message = result.Success
            ? string.Empty
            : result.Message;

        return result.Success;
    }

    private void NotifyChange(Guid? Uid = null)
    {
        if (Uid is not null)
            this.Notifier?.NotifyRecordChanged(this, Uid ?? Guid.Empty);

        this.Notifier?.NotifyListUpdated(this);
    }
}
