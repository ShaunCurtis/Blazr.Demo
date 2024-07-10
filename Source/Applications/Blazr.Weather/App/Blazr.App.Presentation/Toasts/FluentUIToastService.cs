/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.FluentUI.AspNetCore.Components;

namespace Blazr.App.Presentation.Toasts;

public class FluentUIToastService : IAppToastService
{
    private IToastService _toastService;
    private int _defaultTimeOut = 20;

    public FluentUIToastService(IToastService toastService)
    {
        _toastService = toastService;
    }

    public void ShowError(string message, TimeSpan? timeout = null)
    {
        var timespan = timeout ?? TimeSpan.FromSeconds(_defaultTimeOut);
        _toastService.ShowError(message);
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
