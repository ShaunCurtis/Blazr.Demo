# Solution Base Compinents

I'm not a believer in one-size-fits-all when it comes to components.  I do use `ComponentBase`, but I use several other implementations.

## BlazrComponentBase

This is a lightly modified version of `ComponentBase`.  The principle modifications are around the render process and made so that I implement templating with inheritance.  It's the base component for most of the forms.

### Switching fields from private to protected

The following fields have changed:

```csharp
protected RenderFragment renderFragment;
protected bool initialized;
protected bool hasNeverRendered = true;
protected bool hasPendingQueuedRender;
```

All have changed so that overridden methods in inheriting classes have access to them.

### New Properties

```csharp
protected virtual RenderFragment ComponentRenderTree => (builder) => BuildRenderTree(builder);
```

This can be overridden and by default maps to `BuildRenderTree`.

### New Method

```csharp
public BlazrComponentBase()
{
    renderFragment = builder =>
    {
        hasPendingQueuedRender = false;
        hasNeverRendered = false;
        builder.AddContent(0, ComponentRenderTree);
    };
}
```

The base new method now builds out `renderFragment` based on `ComponentRenderTree` rather than `BuildRenderTree`.  This is important because We've  decoupling the render fragment used to build the component from the render fragment method populated by the Razor compiler.  We'll look at the implications of making this change later in this article.

### Reworking StateHasChanged

What was `StateHasChanged` has become `Render`.

```
internal protected void Render()
{
    if (hasPendingQueuedRender)
        return;

    if (hasNeverRendered || ShouldRender() || _renderHandle.IsRenderingOnMetadataUpdate)
    {
        hasPendingQueuedRender = true;

        try
        {
            _renderHandle.Render(renderFragment);
        }
        catch
        {
            hasPendingQueuedRender = false;
            throw;
        }
    }
}
```

`StateHasChanged` now looks like this ensuring any calls to it are invoked on the UI Context thread.

```
protected void StateHasChanged()
    => _renderHandle.Dispatcher.InvokeAsync(Render);
```

## BlazrOwningComponentBase

These are the same as the `OwningComponentBase` implementations, but inherit from `BlazrComponentBase`.

## UIBase

`UIBase` is the base component used for most of the UI components.  It's a leaner and meaner than `ComponentBase`.  It has no lifecycle events such on `OnInitialized` or `OnAfterRender`.  It only renders on parameter changes.  There is also no automated rendering on UI events.  You need to manually call `Render` or `StateHasChanged`.

It's a root inheritance class that implements `IComponent`.

```csharp
public abstract class UIBase : IComponent
```

The internal fields are similar to `BlazrComponentBase`.  They control the render process.

```csharp
protected RenderFragment renderFragment;
protected internal RenderHandle renderHandle;
private bool _hasPendingQueuedRender = false;
protected internal bool hasNeverRendered = true;
protected bool initialized;
protected bool show = true;
```

There are three declared *Parameters* that are common to most UI components:

```csharp
[Parameter] public RenderFragment? ChildContent { get; set; }
[Parameter] public bool Hidden { get; set; } = false;
[Parameter] public string Class { get; set; } = String.Empty;
```

The new method sets `renderFragment` caching the anonymous function for performance.  The fragment sets the control booleans and then checks if it's allowed to render.

 - `Hidden` is a `Parameter` providing display control externally.
 - `show` is the internal bool used to control display.


 ```csharp
    public UIBase()
    {
        this.renderFragment = builder =>
        {
            _hasPendingQueuedRender = false;
            hasNeverRendered = false;
            if (!this.Hidden && this.show)
                this.BuildRenderTree(builder);
        };
    }
```

`BuildRenderTree` is required by the Razor compiler.  It's were it places the code block it's build from the Razor markup. 

```csharp
    protected virtual void BuildRenderTree(RenderTreeBuilder builder) { }
```

`Render` almost does what it says.  It g
queues `renderFragment` onto the Renderer's render queue.  `StateHasChanged` ensures that any render request is run on the UI context thread.

```csharp
    internal protected void Render()
    {
        if (_hasPendingQueuedRender)
            return;

        _hasPendingQueuedRender = true;

        try
        {
            renderHandle.Render(this.renderFragment);
        }
        catch
        {
            _hasPendingQueuedRender = false;
            throw;
        }
    }

    protected void StateHasChanged()
        => renderHandle.Dispatcher.InvokeAsync(Render);
```

We are now down to the `IComponent` implementations.

`Attach` captures the `renderHandle`.

```csharp
    public void Attach(RenderHandle renderHandle)
        => this.renderHandle = renderHandle;
```

`SetParameteraAsync`:

1. Sets the parameters.
2. Calls `ShouldRenderOnParameterChange` which provides a method we can use to manually check parameter equality.  The base implementation simply returns `true`.
3. Checks if we should render or this is the first render.
4. Sets `initialized` to `true`.

```csharp
    public virtual Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        var shouldRender = this.ShouldRenderOnParameterChange(initialized);

        if (hasNeverRendered || shouldRender)
            this.Render();

        this.initialized = true;

        return Task.CompletedTask;
    }

    protected virtual bool ShouldRenderOnParameterChange(bool initialized)
        => true;
```

## UIComponentBase

`UIComponentBase` inherts from `UIBase` and adds a single liefcyle event called `OnParametersChangedAsync`.

```csharp
public abstract class UIComponentBase : UIBase
{
    protected virtual Task OnParametersChangedAsync(bool firstRender)
        => Task.CompletedTask;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        var shouldRender = this.ShouldRenderOnParameterChange(initialized);

        if (hasNeverRendered || shouldRender)
        {
            await this.OnParametersChangedAsync(!initialized);
            this.Render();
        }

        this.initialized = true;
    }
}
```

### UIBlock

`UIBlock` is a base implementation of `UIBase`.   It's a simple Tag wrapper to content.  It's main purpose is to provide base functionality for inheriting classes.

```csharp
namespace Blazr.UI;
public class UIBlock : UIBase
{
    [Parameter] public bool Disabled { get; set; } = false;
    [Parameter] public string? Tag { get; set; }

    protected virtual string HtmlTag => this.Tag ?? "div";
    protected virtual CSSBuilder CssBuilder => new CSSBuilder().AddClass(this.Class);
    protected string CssClass => this.CssBuilder.Build();

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, this.HtmlTag);
        builder.AddAttributeIfNotEmpty(2, "class", this.CssClass);
        builder.AddAttributeIfTrue(this.Disabled, 3, "disabled");
        builder.AddContentIfNotNull(5, this.ChildContent);
        builder.CloseElement();
    }
}
``` 

### UIComponent

`UIComponent` is a base implementation of `UIComponentBase`, similar to `UIBlock` above.

```csharp
namespace Blazr.UI;
public class UIComponent : UIComponentBase
{
    [Parameter] public bool Disabled { get; set; } = false;
    [Parameter] public string? Tag { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> ClickEvent { get; set; }

    protected virtual string HtmlTag => this.Tag ?? "div";
    protected virtual CSSBuilder CssBuilder => new CSSBuilder().AddClass(this.Class);
    protected string CssClass => this.CssBuilder.Build();

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, this.HtmlTag);
        builder.AddAttributeIfNotEmpty(2, "class", this.CssClass);
        builder.AddAttributeIfTrue(this.Disabled, 3, "disabled");
        builder.AddAttributeIfTrue(ClickEvent.HasDelegate, 4, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ClickEvent));
        builder.AddContentIfNotNull(5, this.ChildContent);
        builder.CloseElement();
    }
}
```

#### RenderTreeBuilder Extensions

methods such as `AddAttributeIfTrue` and `AddContentIfNotNull` are extension methods the solution adds to `RenderTreeBuilder`.  You can find all the extensions in the `RenderTreeBuilderExtensions` class in *Blazor.UI*.

