/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public class StandardEditService<TRecord, TEditRecord, TEntity>
    : BaseViewService<TRecord, TEntity>, IEditService<TRecord, TEditRecord, TEntity>
    where TRecord : class, new()
    where TEditRecord : class, IEditRecord<TRecord>, new()
    where TEntity : class, IEntity
{
    protected string AddPolicy = "IsEditorPolicy";
    protected string EditPolicy = "IsEditorPolicy";
    protected string DeletePolicy = "IsEditorPolicy";
    protected string ReadPolicy = "IsViewerPolicy";

    public TEditRecord EditModel { get; private set; } = new TEditRecord();

    public bool IsNewRecord => this.EditModel.IsNew;

    public StandardEditService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService)
        : base(dataBroker, notifier, authenticationState, authorizationService)
    { }

    public async ValueTask<bool> LoadRecordAsync(Guid Id)
    {
        this.Message = string.Empty;

        if (Id != Guid.Empty)
        {
            var result = await this.DataBroker.ExecuteAsync<TRecord>(new RecordQuery<TRecord>(Id));

            if (result.Success && result.Record is not null)
            {

                if (!await this.CheckRecordAuthorization(result.Record, this.ReadPolicy))
                {
                    this.EditModel.Load(new TRecord());
                    return false;
                }

                this.EditModel.Load(result.Record);
                return true;
            }

            this.Message = $"Unable to retrieve record with Id : {Id.ToString()}";
            return false;
        }
        else
            await GetNewRecordAsync(null);

        return true;
    }

    public ValueTask GetNewRecordAsync(TRecord? record)
    {
        this.EditModel.Load(record ?? new TRecord());
        return ValueTask.CompletedTask;
    }

    public async ValueTask<bool> AddRecordAsync()
    {
        this.Message = string.Empty;

        if (!await this.CheckAuthorization(this.AddPolicy))
            return false;

        if (!EditModel.IsNew)
        {
            this.Message = "You can't add an existing record";
            return false;
        }

        var record = EditModel.AsNewRecord;
        var command = new AddRecordCommand<TRecord>(record);
        var result = await this.DataBroker.ExecuteAsync<TRecord>(command);

        if (!result.Success)
        {
            this.Message = "Failed to add the record";
            return false;
        }

        this.EditModel.Load(record);
        this.NotifyChange(EditModel.Uid);
        return true;
    }

    public async ValueTask<bool> UpdateRecordAsync()
    {
        this.Message = string.Empty;

        var record = EditModel.Record;

        if (!await this.CheckRecordAuthorization(this.EditModel.CleanRecord, this.EditPolicy))
            return false;

        var command = new UpdateRecordCommand<TRecord>(record);
        var result = await this.DataBroker.ExecuteAsync<TRecord>(command);

        if (!result.Success)
        {
            this.Message = "Failed to update the record";
            return false;
        }

        this.EditModel.Load(record);
        this.NotifyChange(EditModel.Uid);
        return true;
    }

    public async ValueTask<bool> DeleteRecordAsync()
    {
        if (!await this.CheckRecordAuthorization( this.EditModel.CleanRecord ,this.DeletePolicy))
            return false;

        if (this.EditModel.Record is null)
        {
            this.Message = "No record to delete";
            return false;
        }

        // make sure we have the original record data
        this.EditModel.Reset();

        var record = EditModel.Record;
        var id = EditModel.Uid;
        var command = new DeleteRecordCommand<TRecord>(record);

        var result = await this.DataBroker.ExecuteAsync<TRecord>(command);

        if (!result.Success)
        {
            this.Message = "Failed to update the record";
            return false;
        }
        this.EditModel.Load(new TRecord());
        this.NotifyChange(id);
        return true;
    }

    private void NotifyChange(Guid? Uid = null)
    {
        if (Uid is not null)
            this.Notifier?.NotifyRecordChanged(this, Uid ?? Guid.Empty);

        this.Notifier?.NotifyListUpdated(this);
    }
}
