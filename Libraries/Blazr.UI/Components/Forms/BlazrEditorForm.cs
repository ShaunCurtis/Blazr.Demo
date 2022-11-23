using Microsoft.AspNetCore.Components.Routing;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

public abstract partial class BlazrEditorForm<TEditContext, TRecord, TEntity>
    : Blazr_Form<TEntity>
    where TEditContext : class, IRecordEditContext<TRecord>, IEditContext, new()
        where TEntity : class, IEntity
        where TRecord : class, new()
{
    protected readonly BlazrFormMessage FormMessage = new();
    protected bool isConfirmDelete = false;
    protected IEditService<TEditContext, TRecord> Service = default!;
    protected bool OverrideNavigationLock;

    // Exposing underlying properties from the EditContext
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
        this.Service = ActivatorUtilities.GetServiceOrCreateInstance<IEditService<TEditContext, TRecord>>(serviceProvider);
        await this.Service.LoadRecordAsync(Id);
        this.Service.EditModel.FieldChanged += OnFieldChanged;
        var NavDispose = this.NavManager.RegisterLocationChangingHandler(this.OnLocationChanging);

        if (!string.IsNullOrWhiteSpace(this.EntityUIService.SingleTitle))
            this.FormTitle = $"{this.EntityUIService.SingleTitle} Editor";

        this.LoadState = ComponentState.Loaded;
        await base.SetParametersAsync(ParameterView.Empty);
    }

    private ValueTask OnLocationChanging(LocationChangingContext context )
    {
        if (!OverrideNavigationLock && IsDirty)
            context.PreventNavigation();

        return ValueTask.CompletedTask;
    }

    private void OnFieldChanged(object? sender, string? fieldName)
    {
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

    protected virtual Task<TRecord> GetNewRecord()
        => Task.FromResult(new TRecord());

    protected void ClearConfirmDelete()
        => this.isConfirmDelete = false;

    protected void ShowConfirmDelete()
        => this.isConfirmDelete = true;

    protected void OnRecordChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    protected async void ExitWithoutSaving()
    {
        this.OverrideNavigationLock = true;
        await DoExit();
    }

    public virtual void Dispose()
    {
        if (this.Service is not null)
            this.Service.EditModel.FieldChanged -= OnFieldChanged;
    }
}
