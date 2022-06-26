/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Routing;

public class CoreNavigationManager : NavigationManager, IBlazrNavigationManager
{
    private NavigationManager _baseNavigationManager;

    public CoreNavigationManager(NavigationManager? baseNavigationManager)
    {
        _baseNavigationManager = baseNavigationManager!;
        base.Initialize(_baseNavigationManager!.BaseUri, _baseNavigationManager.Uri);
        _baseNavigationManager.LocationChanged += OnBaseLocationChanged;
    }

    protected override void NavigateToCore(string uri, bool forceLoad)
        => _baseNavigationManager.NavigateTo(uri, forceLoad);

    protected override void EnsureInitialized()
        => base.Initialize(_baseNavigationManager.BaseUri, _baseNavigationManager.Uri);

    private void OnBaseLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        this.Uri = e.Location;
        this.NotifyLocationChanged(e.IsNavigationIntercepted);
    }
}

