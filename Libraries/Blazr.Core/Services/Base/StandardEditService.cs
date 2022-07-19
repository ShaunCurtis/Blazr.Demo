/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public class StandardEditService<TRecord, TEditRecord, TEntity>
    : IEditService<TRecord, TEditRecord, TEntity>
    where TRecord : class, new()
    where TEditRecord : class, IEditRecord<TRecord>, new()
    where TEntity : class, IEntity
{
    protected readonly ICQSDataBroker DataBroker;
    protected INotificationService<TEntity> Notifier;
    protected AuthenticationStateProvider AuthenticationStateProvider;
    protected IAuthorizationService AuthorizationService;
    protected string AddPolicy = "IsEditorPolicy";
    protected string EditPolicy = "IsEditorPolicy";
    protected string DeletePolicy = "IsEditorPolicy";

    public TEditRecord EditModel { get; private set; } = new TEditRecord();

    public bool IsNewRecord => this.EditModel.IsNew;

    public string? Message { get; protected set; }

    public StandardEditService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService)
    {
        this.DataBroker = dataBroker;
        this.Notifier = notifier;
        this.AuthenticationStateProvider = authenticationState;
        this.AuthorizationService = authorizationService;
    }

    public void SetNotificationService(INotificationService<TEntity> service)
        => this.Notifier = service;

    public async ValueTask LoadRecordAsync(Guid Id)
    {
        this.Message = String.Empty;
        if (Id != Guid.Empty)
        {
            var result = await this.DataBroker.ExecuteAsync<TRecord>(new RecordGuidKeyQuery<TRecord>(Id));

            if (result.Success && result.Record is not null)
            {
                this.EditModel.Load(result.Record);
                return;
            }

            this.Message = $"Unable to retrieve record with Id : {Id.ToString()}";
            return;
        }
        else
            await GetNewRecordAsync(null);
    }

    public ValueTask GetNewRecordAsync(TRecord? record)
    {
        this.EditModel.Load(record ?? new TRecord());
        return ValueTask.CompletedTask;
    }

    public async ValueTask<bool> AddRecordAsync()
    {
        this.Message = String.Empty;

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
        this.NotifyChange(EditModel.Id);
        return true;
    }

    public async ValueTask<bool> UpdateRecordAsync()
    {
        var record = EditModel.Record;

        if (!await this.CheckAuthorization(this.EditPolicy))
            return false;

        var command = new UpdateRecordCommand<TRecord>(record);
        var result = await this.DataBroker.ExecuteAsync<TRecord>(command);

        if (!result.Success)
        {
            this.Message = "Failed to update the record";
            return false;
        }

        this.EditModel.Load(record);
        this.NotifyChange(EditModel.Id);
        return true;
    }

    public async ValueTask<bool> DeleteRecordAsync()
    {
        if (!await this.CheckAuthorization(this.DeletePolicy))
            return false;

        if (this.EditModel.Record is null)
        {
            this.Message = "No record to delete";
            return false;
        }

        // make sure we have the original record data
        this.EditModel.Reset();

        var record = EditModel.Record;
        var id = EditModel.Id;
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

    private async ValueTask<bool> CheckAuthorization(string policyName)
    {
        // Check the Authorization policy - can the current user ass a record 
        var authstate = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
        var result = await this.AuthorizationService.AuthorizeAsync(authstate.User, null, policyName);

        if (!result.Succeeded)
            this.Message = "You don't have permissions to add/delete/update this record";

        return result.Succeeded;
    }

    private void NotifyChange(Guid? Uid = null)
    {
        if (Uid is not null)
            this.Notifier?.NotifyRecordChanged(this, Uid ?? Guid.Empty);

        this.Notifier?.NotifyListUpdated(this);
    }
}
