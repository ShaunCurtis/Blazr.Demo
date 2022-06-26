/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.Routing;

public interface IBlazrNavigationManager
{
    public string Uri { get; }

    public string BaseUri { get; }

    public event EventHandler<LocationChangedEventArgs> LocationChanged;

    public string ToBaseRelativePath(string uri);

    public void NavigateTo(string uri, bool forceLoad);

    public void NavigateTo(string uri, bool forceLoad = false, bool replace = false);

    public void NavigateTo(string uri, NavigationOptions options);
}

