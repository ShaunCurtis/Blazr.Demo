/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Routing;

public class PageLocker : ComponentBase, IDisposable
{
    [Inject] private IJSRuntime? _js { get; set; }

    [Inject] private IBlazrNavigationManager? _navManager { get; set; }

    private BlazrNavigationManager? NavManager => (_navManager is BlazrNavigationManager) ? _navManager as BlazrNavigationManager : null;

    protected override void OnInitialized()
    {
        if (this.NavManager is not null)
            NavManager.LockStateChanged += OnLockStateChanged;
    }

    private void OnLockStateChanged(object? sender, LockStateEventArgs e)
        => this.SetPageExitCheck(e.State);

    private void SetPageExitCheck(bool state)
    {
        // Pass the Js code a reference to this instance so it can call AgentExitAttempted
        // if the user tries to exit whilst thw page is locked
        var objref = DotNetObjectReference.Create(this);
        _js!.InvokeAsync<bool>("blazr_setEditorExitCheck", objref , state);
    }

    [JSInvokable]
    public Task AgentExitAttempt()
    {
        NavManager?.NotifyBrowserExitAttempt(this);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (this.NavManager is not null)
            NavManager.LockStateChanged -= OnLockStateChanged;
    }
}

