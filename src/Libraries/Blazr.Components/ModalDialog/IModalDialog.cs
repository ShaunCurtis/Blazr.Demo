/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components;

public interface IModalDialog
{
    public ModalOptions Options { get; }

    public bool IsActive { get; }

    public bool Display { get; }

    public Task<ModalResult> ShowAsync<TModal>(ModalOptions options) where TModal : IComponent;

    public Task<ModalResult> ShowAsync(Type control, ModalOptions options);

    public Task<bool> SwitchAsync<TModal>(ModalOptions options) where TModal : IComponent;

    public Task<bool> SwitchAsync(Type control, ModalOptions options);

    public void Dismiss();

    public void Close(ModalResult result);

    public void Update(ModalOptions? options = null);
}

