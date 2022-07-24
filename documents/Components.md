# Components

The Blazor UI is a set of components organised into a Render Tree.  In the classic setup App.Razor is the root component.

Components must implement the `IComponent` interface.

Here's about as simple as you can make a functional component:

```csharp
public class SimplestComponent : IComponent
{
    private RenderHandle _handle;

    [Parameter] public string Name { get; set; } = String.Empty;

    public void Attach(RenderHandle renderHandle)
        => _handle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        _handle.Render((builder =>
           {
               builder.AddMarkupContent(0, $"<h2>Hello {this.Name}</h2>");
           }
            ));
        return Task.CompletedTask;
    }
}
```

While this may look trivial, the class below is a fully functional component that builds out a Bootstrap success alert.

```csharp
public class Alert : IComponent
{
    private RenderHandle _handle;

    [Parameter, EditorRequired] public RenderFragment? ChildContent { get; set; }

    public void Attach(RenderHandle renderHandle)
        => _handle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        _handle.Render((builder =>
           {
               builder.OpenElement(0, "div");
               builder.AddAttribute(1, "class", "alert alert-success");
               builder.AddContent(2, ChildContent);
               builder.CloseComponent();
           }
            ));
        return Task.CompletedTask;
    }
}
```