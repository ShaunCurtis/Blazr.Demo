/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract partial class BlazrEditorForm<TRecord, TEditRecord, TEntity>
    : BlazrForm<IEditService<TRecord, TEditRecord, TEntity>, TEntity>, IDisposable, IHandleEvent, IHandleAfterRender
    where TRecord : class, new()
    where TEditRecord : class, IEditRecord<TRecord>, new()
    where TEntity : class, IEntity
{
    protected EditContext editContext = default!;

    protected BlazrNavigationManager? blazrNavManager => NavManager is BlazrNavigationManager ? NavManager as BlazrNavigationManager : null;

    protected string? alertMessage;
    protected string alertColour = "alert-info";
    protected int alertTimeOut = 0;
    protected Guid alertId;
    protected bool isConfirmDelete = false;

    protected bool IsModal => this.Modal != null;

    protected bool IsDirty => this.Service.EditModel.IsDirty;

    protected bool IsNew => this.Service.EditModel.IsNew;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        await PreLoadRecordAsync();
        if (isNew)
        {
            if (!string.IsNullOrWhiteSpace(this.EntityUIService.SingleTitle))
                this.FormTitle = $"{this.EntityUIService.SingleTitle} Editor";

            this.Service.SetServices(this.SPAServiceProvider);

            this.LoadState = ComponentState.Loading;

            if (Id != Guid.Empty)
                await this.Service.LoadRecordAsync(Id);
            else
                await this.Service.GetNewRecordAsync(await GetNewRecord());

            this.editContext = new EditContext(this.Service.EditModel);
            this.editContext.OnFieldChanged += FieldChanged;

            if (this.blazrNavManager is not null)
            {
                this.blazrNavManager.BrowserExitAttempted += FailedExitAttempt;
                this.blazrNavManager.NavigationEventBlocked += FailedRoutingAttempt;
            }

            this.LoadState = ComponentState.Loaded;
        }

        await base.SetParametersAsync(ParameterView.Empty);
        isNew = false;
    }

    protected virtual Task<TRecord> GetNewRecord()
        => Task.FromResult(new TRecord());

    private void FieldChanged(object? sender, FieldChangedEventArgs e)
    {
        this.blazrNavManager?.SetLockState(this.IsDirty);
        this.isConfirmDelete = false;
        this.InvokeAsync(StateHasChanged);
    }

    private void FailedRoutingAttempt(object? sender, EventArgs e)
        => this.SetMessage("You can't navigate away from this form with unsaved data", "alert-warning");

    private void FailedExitAttempt(object? sender, EventArgs e)
    => this.SetMessage("You attempted to exit the application", "alert-warning");

    protected void ResetRecord()
    {
        this.Service.EditModel.Reset();
        this.SetMessage("Fields reset to database values", "alert-info");
    }

    protected async Task<bool> SaveRecord()
    {
        var result = false;
        if (this.editContext.Validate())
        {
            result = await this.Service.UpdateRecordAsync();
            this.blazrNavManager?.SetLockState(this.IsDirty);
            if (result)
                this.SetMessage("Record Saved", "alert-success");
            else
                this.SetMessage(this.Service.Message ?? "Problem saving record", "alert-danger");
        }
        else
            this.SetMessage("There are validation problems", "alert-danger");

        return result;
    }

    protected virtual async Task SaveRecordAndExit()
    {
        if (await this.SaveRecord())
            await DoExit();
    }

    protected virtual async Task<bool> AddRecord()
    {
        var hasSaved = false;
        if (this.editContext.Validate())
        {
            var result = await this.Service.AddRecordAsync();
            this.blazrNavManager?.SetLockState(this.IsDirty);
            if (result)
            {
                this.SetMessage("Record Added", "alert-success");
                await this.Service.LoadRecordAsync(this.Service.EditModel.Uid);
                hasSaved = true;
            }
            else
                this.SetMessage(this.Service.Message ?? "Problem adding record", "alert-danger");
        }
        else
            this.SetMessage("There are validation problems", "alert-danger");

        return hasSaved;
    }

    protected virtual async Task AddRecordAndExit()
    {
        if (await this.AddRecord())
            await DoExit();
    }

    protected void ClearConfirmDelete()
        => this.isConfirmDelete = false;

    protected void ShowConfirmDelete()
        => this.isConfirmDelete = true;

    protected void OnRecordChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    protected void OnEditStateChanged(object? sender, EditStateEventArgs e)
    {
        this.blazrNavManager?.SetLockState(e.IsDirty);
        this.StateHasChanged();
    }

    protected async void ExitWithoutSaving()
    {
        this.blazrNavManager?.SetLockState(false);
        await DoExit();
    }

    protected void SetMessage(string message, string colour)
    {
        this.alertMessage = message;
        this.alertColour = colour;
        this.alertTimeOut = 0;
        this.alertId = Guid.NewGuid();
        this.StateHasChanged();
    }

    public virtual void Dispose()
    {
        if (this.editContext is not null)
            this.editContext.OnFieldChanged -= FieldChanged;
        if (this.blazrNavManager is not null)
        {
            this.blazrNavManager.BrowserExitAttempted -= FailedExitAttempt;
            this.blazrNavManager.NavigationEventBlocked -= FailedRoutingAttempt;
        }
    }
}
