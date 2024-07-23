/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation.Toasts;

public interface IAppToastService
{
    public void ShowWarning(string Message, TimeSpan? timeout = null);
    public void ShowSuccess(string Message, TimeSpan? timeout = null);
    public void ShowError(string Message, TimeSpan? timeout = null);
}
