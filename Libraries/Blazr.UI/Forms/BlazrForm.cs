/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class BlazrForm<TService, TEntity>
    : BlazrOwningComponentBase<TService>, IDisposable, IHandleEvent, IHandleAfterRender
    where TService : class
    where TEntity : class, IEntity
{
    private bool _isNew = true;
    protected string FormTitle = "Record Viewer";

    /// <summary>
    /// Id for the record
    /// </summary>
    [Parameter] public Guid Id { get; set; }

    /// <summary>
    /// Pick up a Cascaded IModalDialog if one is configured on the parent
    /// </summary>
    [CascadingParameter] public IModalDialog? Modal { get; set; }

    // Get all the DI Services we need
    [Inject] protected NavigationManager NavManager { get; set; } = default!;

    [Inject] protected ModalService ModalService { get; set; } = default!;

    [Inject] protected INotificationService<TEntity> NotificationService { get; set; } = default!;

    [Inject] protected IEntityService<TEntity> EntityService { get; set; } = default!;

    [Inject] protected IEntityUIService<TEntity> EntityUIService { get; set; } = default!;

    [Inject] protected IServiceProvider SPAServiceProvider { get; set; } = default!;

    /// <summary>
    /// The component state
    /// </summary>
    public ComponentState LoadState { get; protected set; } = ComponentState.New;

    /// <summary>
    /// Method in the component event chain prior to loading the record 
    /// </summary>
    /// <returns></returns>
    protected virtual Task PreLoadRecordAsync()
        => Task.CompletedTask;

    /// <summary>
    /// Method to load the Record
    /// </summary>
    /// <param name="render"></param>
    /// <returns></returns>
    private Task LoadRecordAsync(bool render = false)
        => Task.CompletedTask;

    /// <summary>
    /// Default Exit for buttons
    /// </summary>
    protected abstract void Exit();

    async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
    {
        await callback.InvokeAsync(arg);
        Render();
    }

    Task IHandleAfterRender.OnAfterRenderAsync()
        => Task.CompletedTask;
}
