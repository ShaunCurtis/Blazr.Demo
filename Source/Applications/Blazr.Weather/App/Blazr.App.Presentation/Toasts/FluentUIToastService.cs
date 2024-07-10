/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.FluentUI.AspNetCore.Components;

namespace Blazr.App.Presentation.Toasts;

/// <summary>
/// Basic facade into the FluentUI toaster
/// Uses the default Timeout due to bugs in setting the timeout
/// </summary>
public class FluentUIToastService : IAppToastService
{
    private IToastService _toastService;
    private int _defaultTimeOut = 10;

    public FluentUIToastService(IToastService toastService)
    {
        _toastService = toastService;
    }

    public void ShowError(string message, TimeSpan? timeout = null)
    {
        var timespan = timeout ?? TimeSpan.FromSeconds(_defaultTimeOut);
        _toastService.ShowError(message, timespan.Seconds);
    }

    public void ShowSuccess(string message, TimeSpan? timeout = null)
    {
        var timespan = timeout ?? TimeSpan.FromSeconds(_defaultTimeOut);
        _toastService.ShowSuccess(message);
    }

    public void ShowWarning(string message, TimeSpan? timeout = null)
    {
        var timespan = timeout ?? TimeSpan.FromSeconds(_defaultTimeOut);
        _toastService.ShowWarning(message);
    }
}
