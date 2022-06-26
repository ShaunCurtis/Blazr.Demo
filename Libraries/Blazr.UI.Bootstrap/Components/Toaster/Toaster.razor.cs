/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Core.Toaster;

namespace Blazr.UI.Bootstrap;

public partial class Toaster : ComponentBase, IDisposable
{
    [Inject] private ToasterService? _toasterService { get; set; }

    private ToasterService toasterService => _toasterService!;

    protected override void OnInitialized()
    { 
        this.toasterService.ToasterChanged += ToastChanged;
        this.toasterService.ToasterTimerElapsed += ToastChanged;
    }

    private void ClearToast(Toast toast)
        => toasterService.ClearToast(toast);

    private void ToastChanged(object? sender, EventArgs e)
        => this.InvokeAsync(this.StateHasChanged);

    public void Dispose()
    { 
        this.toasterService.ToasterChanged -= ToastChanged;
        this.toasterService.ToasterTimerElapsed -= ToastChanged;
    }

    private string toastCss(Toast toast)
    {
        var colour = Enum.GetName(typeof(MessageColour), toast.MessageColour)?.ToLower();
        return toast.MessageColour switch
        {
            MessageColour.Light => "bg-light",
            _ => $"bg-{colour} text-white"
        };
    }
}

