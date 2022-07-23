/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI;

/// <summary>
/// Base minimum footprint component for building simple UI Components
/// No events or StateHasChanged
/// </summary>
public abstract class UIBase : IComponent
{
    protected RenderFragment _renderFragment;
    protected internal RenderHandle _renderHandle;
    protected bool _hasPendingQueuedRender = false;
    protected bool _show = true;

/// <summary>
/// Content to render within the component
/// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Collection of attributes assigned to the componebnt
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)] public IDictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Parameter to control the display of the component
    /// </summary>
    [Parameter] public bool Hidden { get; set; } = false;

    /// <summary>
    /// A list of attributes to be removed from the UserAttributes before render
    /// </summary>
    protected virtual List<string> UnwantedAttributes { get; set; } = new List<string>();

    /// <summary>
    /// Attributes to be added to the primary component element
    /// It's the list of UserAttributes with any Unwanted attributes removed
    /// </summary>
    protected Dictionary<string, object> SplatterAttributes
    {
        get
        {
            var list = new Dictionary<string, object>();
            foreach (var item in UserAttributes)
            {
                if (!UnwantedAttributes.Any(item1 => item1.Equals(item.Key)))
                    list.Add(item.Key, item.Value);
            }
            return list;
        }
    }

    /// <summary>
    /// New method
    /// caches a copy of the Render code
    /// Detects if the component shoud be rendered and if not doesn't render ant content
    /// </summary>
    public UIBase()
    {
        _renderFragment = builder =>
        {
            _hasPendingQueuedRender = false;
            if (!this.Hidden && _show)
                BuildRenderTree(builder);
        };
    }

    /// <summary>
    /// Default Render method required by Razor to compile the Razor markup to.
    /// </summary>
    /// <param name="builder"></param>
    protected virtual void BuildRenderTree(RenderTreeBuilder builder) { }

    /// <summary>
    /// Internal method to queue the component Render Fragment onto the Renderer's Render Queue
    /// Only adds it if there are no other copies already queued
    /// </summary>
    internal protected void Render()
    {
        if (_hasPendingQueuedRender)
            return;

        _hasPendingQueuedRender = true;

        try
        {
            _renderHandle.Render(_renderFragment);
        }
        catch
        {
            _hasPendingQueuedRender = false;
            throw;
        }
    }

    /// <summary>
    /// Method to invoke an Action on the UI Context thread
    /// </summary>
    /// <param name="workItem"></param>
    /// <returns></returns>
    protected Task InvokeAsync(Action workItem)
        => _renderHandle.Dispatcher.InvokeAsync(workItem);

    /// <summary>
    /// Method to invoke a Task Method on the UI Context thread
    /// </summary>
    /// <param name="workItem"></param>
    /// <returns></returns>
    protected Task InvokeAsync(Func<Task> workItem)
        => _renderHandle.Dispatcher.InvokeAsync(workItem);


    /// <summary>
    ///  IComponent implementation
    ///  Gets and saves the provided RenderHandle
    /// </summary>
    /// <param name="renderHandle"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Attach(RenderHandle renderHandle)
    {
        if (_renderHandle.IsInitialized)
            throw new InvalidOperationException($"The render handle is already set. Cannot initialize a {nameof(ComponentBase)} more than once.");

        _renderHandle = renderHandle;
    }

    /// <summary>
    ///  IComponent implementation
    /// Called by the Renderer at initialization and whenever any of the requested Parameters change
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public virtual Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        this.Render();
        return Task.CompletedTask;
    }
}

