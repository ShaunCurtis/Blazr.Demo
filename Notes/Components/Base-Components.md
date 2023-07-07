# Base Components

I don't use `ComponentBase`.  You can read why anf what I do in the following published articles.

I'll summarise here what I do.

I have three base components:

1. `BlazorUIBase` is a small footprint basic component with no lifecycle methods and wired UI event handlers.  It's for building standard Html based components.

2. `BlazorControlBase` is a medium footprint component with a single lifecycle method and a single render UI event handler.  It's the workhorse component of the three.

3. `BlazorComponentBase` is the `ComponentBase` equivalent with all the bells and whistles.  I rarely use it.

All three components inherit from a base class that contains the basic boilerplate code all components need.

Here's the code for `BlazorControlBase`:

```csharp
public abstract class BlazrControlBase : BlazrBaseComponent, IComponent, IHandleEvent
{
    public async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        await this.OnParametersSetAsync();
        this.StateHasChanged();
    }

    protected virtual Task OnParametersSetAsync()
        => Task.CompletedTask;  

    async Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem item, object? obj)
    {
        await item.InvokeAsync(obj);
        this.StateHasChanged();
    }
}
```

When you start to use this component there are some important facts to understand.

1. You can't override `SetParametersAsync`, it's not virtual.

2. Everything is done in `OnParametersSetAsync`.  The base component provides two readonly bool properties `Initialized` and `NotInitialized` to use to control logic flow for the first and subsequant times the mwthod is called.

3. There's a simple `IHandleEvent.HandleEventAsync` UI event handler registered that calls `StateHasChanged` after the event handler is called.