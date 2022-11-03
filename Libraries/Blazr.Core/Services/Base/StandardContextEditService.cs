using Blazr.Routing;
using Microsoft.AspNetCore.Components;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public class StandardEditContextService<TEditContext, TRecord, TEntity>
    : BaseViewService<TRecord, TEntity>,
        IContextEditService<TEditContext, TRecord>
    where TRecord : class, new()
    where TEditContext : class, IRecordEditContext<TRecord>, new()
    where TEntity : class, IEntity
{
    public IRecordEditContext<TRecord> EditModel { get; private set; } = new TEditContext();
    protected BlazrNavigationManager? BlazrNavManager => NavigationManager is BlazrNavigationManager ? NavigationManager as BlazrNavigationManager : null;

    public StandardEditContextService(ICQSDataBroker dataBroker, INotificationService<TEntity> notifier, AuthenticationStateProvider authenticationState, IAuthorizationService authorizationService, NavigationManager navigationManager)
        : base(dataBroker, notifier, authenticationState, authorizationService, navigationManager)
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

    public async ValueTask<CommandResult> AddRecordAsync()
    {

        if (!this.EditModel.Validate().IsValid)
            return CommandResult.Failure("There are validation problems");

        this.Message = string.Empty;

        if (!await this.CheckAuthorization(this.AddPolicy))
            return CommandResult.Failure("You don't have permission to update the Record");

        if (!EditModel.IsNew)
            return CommandResult.Failure("You can't add an existing record");

        var record = EditModel.AsNewRecord();
        var command = AddRecordCommand<TRecord>.GetCommand(record);
        var result = await this.DataBroker.ExecuteAsync<TRecord>(command);

        if (!result.Success)
            return result;

        this.EditModel.Load(record);
        this.BlazrNavManager?.SetLockState(false);
        this.NotifyChange(EditModel.Uid);
        return CommandResult.Successful("Record Added");
    }

    public async ValueTask<CommandResult> UpdateRecordAsync()
    {
        if (!this.EditModel.Validate().IsValid)
            return CommandResult.Failure("There are validation problems");

        if (!await this.CheckRecordAuthorization(this.EditModel.CleanRecord, this.EditPolicy))
            return CommandResult.Failure("You don't have permission to update the Record");

        var record = EditModel.Record;

        var command = UpdateRecordCommand<TRecord>.GetCommand(this.EditModel.Record);
        var result = await this.DataBroker.ExecuteAsync<TRecord>(command);

        if (result.Success)
            return result;

        this.EditModel.Load(record);
        this.BlazrNavManager?.SetLockState(false);
        this.NotifyChange(EditModel.Uid);
        return CommandResult.Successful("Record Saved");
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
