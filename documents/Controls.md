# Controls

Controls are base UI components.  Controls build out Markup for the Renderer to render.

The controls are in two libraries.  *Blazor.UI* contains all the CSS agnostic controls.  *Blazor.UI.Bootstrap* contains all the controls that use the Bootstrap CSS framework.

## Examples

Let's look at one to illustrate.  This one changes the `display` CSS setting to show or hide the enclosed block of markup.  Note that there are other ways to do this in Blazor.

```csharp
@namespace Blazr.UI.Bootstrap

@inherits ComponentBase
<div class="@this.Css">
    @ChildContent
</div>
<style>
    .display-on { display: inherit; }
    .display-off { display: none; }
</style>

@code {
    private string Css => this.Display ? "dislpay-on" : "display-off";

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool Display { get; set; }
}
```

It renders a `div` with the `ChildContent` as it's content.  It changes the CSS class based on the `Display` parameter.

Here's another control.  This one extends the functionality of `InputText` to add the option to trigger the data update event on the `onInput` input event rather than the `onChange` event.  It also implements a `IComponentReference` interface that is used to set the focus to this control in edit forms.

```csharp
public class BlazrInputText : InputText, IComponentReference
{
    [Parameter] public bool BindOnInput { get; set; } = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);

        if (!string.IsNullOrWhiteSpace(this.CssClass))
            builder.AddAttribute(2, "class", CssClass);

        builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValueAsString));

        if (BindOnInput)
            builder.AddAttribute(4, "oninput", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
        else
            builder.AddAttribute(5, "onchange", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));

        builder.AddElementReferenceCapture(6, __inputReference => Element = __inputReference);
        builder.CloseElement();
    }
}
```

This control is a class, not a Razor component, and implements the markup using `RenderTreeBuilder` construction methods. The render code is lifted from `InputText` and modifies which change event is wired back to `CurrentValueAsString`.

 ## UIControls

