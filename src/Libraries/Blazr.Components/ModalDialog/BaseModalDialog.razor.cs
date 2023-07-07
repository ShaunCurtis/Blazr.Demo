/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.Components;

public partial class BaseModalDialog : ModalDialogBase, IModalDialog
{
    protected string Width => this.Options.TryGet<string>(ModalOptions.__Width, out string? value) ? $"width:{value}" : string.Empty;

    protected bool ExitOnBackGroundClick => this.Options.TryGet<bool>(ModalOptions.__ExitOnBackGroundClick, out bool value) ? value : false;

    private void OnBackClick()
    {
        if (ExitOnBackGroundClick)
            this.Close(ModalResult.Exit());
    }
}

