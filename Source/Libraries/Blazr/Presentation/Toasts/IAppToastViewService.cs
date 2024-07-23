/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation.Toasts;

/// <summary>
/// Interface used by the UI component to display Toasts
/// </summary>
public interface IAppToastViewService
{
    public event EventHandler? ToastsChanged;
    public IEnumerable<Toast> Toasts { get; }
    public TimeSpan NextToastTimeOut();
    public void DismissToast(Guid id, object? token = null);
}
