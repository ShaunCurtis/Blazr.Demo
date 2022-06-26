/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

public partial class BaseEditForm<TRecord> : OwningComponentBase
{
    protected EditContext? editContent;

    [Parameter] public Guid Id { get; set; } = GuidExtensions.Null;

    [Parameter] public EventCallback ExitAction { get; set; }

    [CascadingParameter] public IModalDialog? Modal { get; set; }

    [CascadingParameter] public Task<AuthenticationState>? AuthTask { get; set; }

    [Inject] protected IBlazrNavigationManager? NavManager { get; set; }

    protected BlazrNavigationManager? BlazrNavManager => NavManager is BlazrNavigationManager ? NavManager as BlazrNavigationManager : null;
    
    [Inject] protected ToasterService? toasterService { get; set; }

    [Inject] protected ResponseMessageStore? ResponseMessageStore { get; set; }

    [Inject] protected IAuthorizationService? AuthorizationService { get; set; }

    public ComponentState LoadState { get; protected set; } = ComponentState.New;

    protected bool IsModal => this.Modal != null;

    protected EditStateContext? editStateContext;

    protected bool IsDirty => editStateContext!.IsDirty;

    protected void OnRecordChanged(object? sender, EventArgs e)
        => this.InvokeAsync(StateHasChanged);

    protected void OnEditStateChanged(object? sender, EditStateEventArgs e)
    {
        this.BlazrNavManager?.SetLockState(e.IsDirty);
        this.InvokeAsync(StateHasChanged);
    }

    protected async void Exit()
        => await DoExit();

    protected async void ExitWithoutSaving()
    {
        this.BlazrNavManager?.SetLockState(false);
        await DoExit();
    }

    protected virtual async Task DoExit()
    {
        // If we're in a modal context, call Close on the cascaded Modal object
        if (this.Modal is not null)
            this.Modal.Close(ModalResult.OK());
        // If there's a delegate registered on the ExitAction, execute it. 
        else if (ExitAction.HasDelegate)
            await ExitAction.InvokeAsync();
        // else fallback action is to navigate to root
        else
            this.BaseExit();
    }

    protected virtual void BaseExit()
        => this.NavManager?.NavigateTo("/");

    protected virtual async Task<bool> CheckAuthorization(object data, string policy)
    {
        var state = await AuthTask!;
        var result = await this.AuthorizationService!.AuthorizeAsync(state.User, data, policy);
        return result.Succeeded;
    }
}
