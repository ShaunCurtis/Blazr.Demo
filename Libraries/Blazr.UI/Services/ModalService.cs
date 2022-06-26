/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class ModalService
{
    public IModalDialog? ModalDialog { get; set; }

    public IModalDialog Modal => ModalDialog!;

    public bool IsModalFree => !this.ModalDialog?.IsActive ?? false;
}
