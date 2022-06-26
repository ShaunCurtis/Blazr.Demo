/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Routing;

public class BlazrNavigationEventArgs : EventArgs
{
    public string Uri { get; set; } = String.Empty;

    public BlazrNavigationEventArgs() { }

    public BlazrNavigationEventArgs(string uri)
        => this.Uri = uri;

    public static BlazrNavigationEventArgs New(string uri)
        => new BlazrNavigationEventArgs(uri);
}

