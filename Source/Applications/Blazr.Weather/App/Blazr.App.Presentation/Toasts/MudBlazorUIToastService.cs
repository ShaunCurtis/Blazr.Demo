/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Presentation.Toasts;
using MudBlazor;

namespace Blazr.App.Presentation.Toasts;

/// <summary>
/// Basic facade into the MudBlazor Snackbar
/// Uses the default Timeout for simplicity
/// </summary>
public class MudBlazorUIToastService : IAppToastService
{
    private ISnackbar _snackbar;

    public MudBlazorUIToastService(ISnackbar snackbar)
    {
        _snackbar = snackbar;
    }

    public void ShowError(string message, TimeSpan? timeout = null)
    {
        _snackbar.Add(message, Severity.Error, c => c.SnackbarVariant = Variant.Filled);
    }

    public void ShowSuccess(string message, TimeSpan? timeout = null)
    {
        _snackbar.Add(message, Severity.Success, c => c.SnackbarVariant = Variant.Filled);
    }

    public void ShowWarning(string message, TimeSpan? timeout = null)
    {
        _snackbar.Add(message, Severity.Warning, c => c.SnackbarVariant = Variant.Filled);
    }
}
