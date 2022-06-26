/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Routing;

public class BlazrNavigationManager : NavigationManager, IBlazrNavigationManager, IDisposable
{
    private NavigationManager _baseNavigationManager;
    private bool _isBlindNavigation = false;

    public bool IsLocked { get; protected set; } = false;

    public event EventHandler<BlazrNavigationEventArgs>? NavigationEventBlocked;
    public event EventHandler<LockStateEventArgs>? LockStateChanged;
    public event EventHandler? BrowserExitAttempted;

    public BlazrNavigationManager(NavigationManager? navigationManager)
    {
        _baseNavigationManager = navigationManager!;
        base.Initialize(navigationManager!.BaseUri, navigationManager.Uri);
        _baseNavigationManager.LocationChanged += OnBaseLocationChanged;
    }

    /// <summary>
    /// Sets the local state of the page
    /// </summary>
    /// <param name="state"></param>
    public void SetLockState(bool state)
    {
        if (state != this.IsLocked)
        {
            this.IsLocked = state;
            this.NotifyLockStateChanged(this, new LockStateEventArgs(this.IsLocked));
        }
    }

    private void OnBaseLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // Check if we are blind navigating - i.e. just resetting the display Uri. - Do nothing
        if (this.BlindNavigation())
            return;

        // Check if Navigation is locked
        if (this.LockedNavigation(e.Location))
            return;

        // Normal Navigation path

        // NOTE: We set the Uri before calling notify location changed, as it will use this uri property in its args.
        this.Uri = e.Location;

        // Trigger the Location Changed event for all listeners including the Router
        this.NotifyLocationChanged(e.IsNavigationIntercepted);

        // Belt and braces to ensure false 
        _isBlindNavigation = false;
    }

    private bool BlindNavigation()
    {
        if (_isBlindNavigation)
        {
            _isBlindNavigation = false;
            return true;
        }
        return false;
    }

    private bool LockedNavigation(string uri)
    {
        // Sets the displayed uri back to the orginal if we're locked.
        if (this.IsLocked)
        {
            // prevents a NavigateTo loop
            _isBlindNavigation = true;
            _baseNavigationManager.NavigateTo(this.Uri, false);
            this.NotifyNavigationEventBlocked(this, new BlazrNavigationEventArgs(uri));
        }
        return this.IsLocked;
    }

    protected override void NavigateToCore(string uri, bool forceLoad)
        => _baseNavigationManager.NavigateTo(uri, forceLoad);
    
    protected override void EnsureInitialized()
    => base.Initialize(_baseNavigationManager.BaseUri, _baseNavigationManager.Uri);

    protected void NotifyNavigationEventBlocked(object? sender, BlazrNavigationEventArgs e)
        => this.NavigationEventBlocked?.Invoke(sender, e);

    protected void NotifyLockStateChanged(object? sender, LockStateEventArgs e)
        => this.LockStateChanged?.Invoke(sender, e);

    public void NotifyBrowserExitAttempt(object? sender)
        => this.BrowserExitAttempted?.Invoke(sender, EventArgs.Empty);

    public void Dispose()
        => _baseNavigationManager.LocationChanged -= OnBaseLocationChanged;
}

