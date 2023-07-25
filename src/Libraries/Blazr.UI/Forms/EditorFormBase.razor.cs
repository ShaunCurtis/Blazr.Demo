/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using FluentValidation;

namespace Blazr.UI;

public abstract partial class EditorFormBase<TRecord, TEditContext, TEntityService, TRecordValidator> : BlazrControlBase, IDisposable
    where TRecord : class, IStateEntity, IEntity, new()
    where TEditContext : class, IBlazrRecordEditContext<TRecord>, new()
    where TEntityService : class, IEntityService
    where TRecordValidator : class, IValidator<TEditContext>, new()
{
    [Inject] protected IBlazrEditPresenter<TRecord, TEditContext> Presenter { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IUIEntityService<TEntityService> UIEntityService { get; set; } = default!;

    [CascadingParameter] private IModalDialog? ModalDialog { get; set; }
    [Parameter] public Guid Uid { get; set; }
    [Parameter] public bool LockNavigation { get; set; } = true;

    protected string exitUrl = "/";

    protected EditFormButtonsOptions editFormButtonsOptions = new();
    protected bool ExitOnSave = true;

    protected bool IsNewRecord => this.Presenter.RecordContext.EntityState.IsNew;

    protected async override Task OnParametersSetAsync()
    {
        if (this.NotInitialized)
        {
            await this.Presenter.LoadAsync(new(Uid));

            this.Presenter.EditContext.OnFieldChanged += OnEditStateMayHaveChanged;
        }
    }

    protected async Task OnSave()
    { 
        await this.Presenter.SaveItemAsync();
        if (this.ExitOnSave)
            await OnExit();
    }

    protected Task OnExit()
    {
        if (this.ModalDialog is null)
            this.NavManager.NavigateTo(this.exitUrl);

        ModalDialog?.Close(new ModalResult());
        return Task.CompletedTask;
    }

    protected void OnEditStateMayHaveChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    protected async Task OnReset()
    {
        await this.Presenter.ResetItemAsync();
        this.Presenter.EditContext.Validate();
    }

    public void Dispose()
    {
        //TODO - why could this be null????
        if (this.Presenter.EditContext is not null)
            this.Presenter.EditContext.OnFieldChanged -= OnEditStateMayHaveChanged;
    }
}
