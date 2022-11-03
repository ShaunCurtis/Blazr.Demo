/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract partial class Blazr_Editor_Form<TEditContext, TRecord, TEntity>
    : UITemplatedComponentBase
    where TEditContext : class, IRecordEditContext<TRecord>, new()
        where TEntity : class, IEntity
        where TRecord : class, new()
{
    /// <summary>
    /// Id for the record
    /// </summary>
    [Parameter] public Guid Id { get; set; }

    /// <summary>
    /// Specify a specific exit mechanism
    /// </summary>
    [Parameter] public EventCallback ExitAction { get; set; }

    /// <summary>
    /// Pick up a Cascaded IModalDialog if we're hosted within a Modal Dialog context
    /// </summary>
    [CascadingParameter] public IModalDialog? Modal { get; set; }

    [Inject] protected IEntityUIService<TEntity> EntityUIService { get; set; } = default!;
    [Inject] private IServiceProvider serviceProvider { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected ModalService ModalService { get; set; } = default!;

    protected readonly BlazrFormMessage FormMessage = new();
    protected bool isConfirmDelete = false;
    protected string FormTitle = "Record Editor";
    protected IContextEditService<TEditContext, TRecord> Service = default!;
    protected BlazrNavigationManager? blazrNavManager => NavManager is BlazrNavigationManager ? NavManager as BlazrNavigationManager : null;
    protected bool IsDirty => this.Service.EditModel.IsDirty;
    protected bool IsNew => this.Service.EditModel.IsNew;


    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (initialized)
        {
            await base.SetParametersAsync(ParameterView.Empty);
            return;
        }

        // is not initialized from here

        this.Service = ActivatorUtilities.GetServiceOrCreateInstance<IContextEditService<TEditContext, TRecord>>(serviceProvider);
        await this.Service.LoadRecordAsync(Id);
        this.Service.EditModel.FieldChanged -= OnFieldChanged;

        if (!string.IsNullOrWhiteSpace(this.EntityUIService.SingleTitle))
            this.FormTitle = $"{this.EntityUIService.SingleTitle} Editor";

        if (this.blazrNavManager is not null)
        {
            this.blazrNavManager.BrowserExitAttempted += OnFailedExitAttempt;
            this.blazrNavManager.NavigationEventBlocked += OnFailedRoutingAttempt;
        }

        await base.SetParametersAsync(ParameterView.Empty);
    }

    protected virtual Task<TRecord> GetNewRecord()
        => Task.FromResult(new TRecord());

    private void OnFieldChanged(object? sender, string? fieldName)
    {
        this.blazrNavManager?.SetLockState(this.IsDirty);
        this.isConfirmDelete = false;
        this.InvokeStateHasChanged();
    }

    private void OnFailedRoutingAttempt(object? sender, EventArgs e)
        => this.FormMessage.SetMessage("You can't navigate away from this form with unsaved data", "alert-warning");

    private void OnFailedExitAttempt(object? sender, EventArgs e)
    => this.FormMessage.SetMessage("You attempted to exit the application", "alert-warning");

    protected void ResetRecord()
    {
        this.Service.EditModel?.Reset();
        this.FormMessage.SetMessage("Fields reset to database values", "alert-info");
    }

    protected async Task<bool> SaveRecord()
    {
        var result = await this.Service.UpdateRecordAsync();
        this.FormMessage.SetMessage(result.Message, result.MessageType);
        return result.Success;
    }

    protected virtual async Task SaveRecordAndExit()
    {
        if (await this.SaveRecord())
            await DoExit();
    }

    protected virtual async Task<bool> AddRecord()
    {
        var result = await this.Service.AddRecordAsync();
        this.FormMessage.SetMessage(result.Message, result.MessageType);
        return result.Success;
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

    protected async void ExitWithoutSaving()
    {
        this.blazrNavManager?.SetLockState(false);
        await DoExit();
    }

    /// <summary>
    /// Default Exit for buttons
    /// </summary>
    protected virtual async void Exit()
        => await DoExit();

    /// <summary>
    /// Exit method that figures out the correct exit method i.e. exit from a modal dialog or exit from a form
    /// </summary>
    /// <returns></returns>
    protected async Task DoExit()
    {
        // If we're in a modal context, call Close on the cascaded Modal object
        if (this.Modal is not null)
        {
            this.Modal.Close(ModalResult.OK());
            return;
        }

        // If there's a delegate registered on the ExitAction, execute it. 
        if (ExitAction.HasDelegate)
        {
            await ExitAction.InvokeAsync();
            return;
        }

        // fallback action is to navigate to root
        this.BaseExit();
    }

    /// <summary>
    /// Exit to the Entity defined Url
    /// </summary>
    protected void BaseExit()
        => this.NavManager?.NavigateTo($"/{this.EntityUIService.Url}");

    public virtual void Dispose()
    {
            this.Service.EditModel.FieldChanged -= OnFieldChanged;

        if (this.blazrNavManager is not null)
        {
            this.blazrNavManager.BrowserExitAttempted -= OnFailedExitAttempt;
            this.blazrNavManager.NavigationEventBlocked -= OnFailedRoutingAttempt;
        }
    }
}
