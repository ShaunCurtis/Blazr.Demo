using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;

namespace Blazr.Components;

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
