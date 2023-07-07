/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

public abstract partial class ViewerFormBase<TRecord, TEntityService> : BlazrControlBase
    where TRecord : class, new()
    where TEntityService : class, IEntityService
{
    [Inject] protected IReadPresenter<TRecord> Presenter { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IUIEntityService<TEntityService> UIEntityService { get; set; } = default!;

    [Parameter] public Guid Uid { get; set; }
    [CascadingParameter] private IModalDialog? ModalDialog { get; set; }

    protected string ExitUrl { get; set; } = "/";

    protected async override Task OnParametersSetAsync()
    {
        if (this.NotInitialized)
            await this.Presenter.LoadAsync(Uid);
    }

    protected Task OnExit()
    {
        if (this.ModalDialog is null)
            this.NavManager.NavigateTo(this.ExitUrl);

        ModalDialog?.Close(new ModalResult());
        return Task.CompletedTask;
    }
}
