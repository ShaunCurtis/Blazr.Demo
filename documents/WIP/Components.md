# Components

Within th solution the overall design goal is to structure the UI into three layers:

## Base Components

The solution uses several base components which are covered in [Base Components](./../Base-Components.md).

### Routes

Also known as a page.  A route is just a component with one of more `Page` attributes.  Routes in general only contain Forms.

Here's a typical route:

```csharp
@page "/weatherforecast/list"
@page "/weatherforecast/pagedlist"
@page "/weatherforecast"
@page "/weatherforecasts"
@attribute [Authorize(Policy = "IsVisitorPolicy")]
@namespace Blazr.App.UI

<WeatherForecastPagedListForm RouteId=RouteId UseModalForms=true class="mt-2"/>

@code {
    private static Guid RouteId = Guid.NewGuid();
}
``` 
 
It contains one component - `WeatherForecastPagedListForm`.  `RouteId` provides a unique Id for this route used in state management.

### Forms

Forms contain a collection of low level components I call controls.  The don't normally contain markup.  They often inherit from base forms that implement most, if not all, functionality in boilerplate code.

A typical View form looks like this:

```csharp
@namespace Blazr.App.UI
@inherits BlazrAppViewerForm<DboWeatherLocation, WeatherLocationEntity>
<UIContainer Size=BootstrapSize.Fluid>
    <UIFormRow>
        <UIColumn Columns=12 MediumColumns=6>
            <FormViewControl Label="Location" Value="this.Service.Record!.Location" ControlType="typeof(InputReadOnlyDisplay)" />
        </UIColumn>
        <UIColumn Columns=12 MediumColumns=6>
            <FormViewControl Label="Unique Record Identity" Value="this.Service.Record!.Uid" ControlType="typeof(InputReadOnlyDisplay)" />
        </UIColumn>
    </UIFormRow>
</UIContainer>

@code {
    protected override RenderFragment BaseContent => (builder) => base.BuildRenderTree(builder);
}
```

### Controls

Controls are the raw building blocks of the UI.  They build markup out into reusable controls.  Thick **DRY** Do not Repeat Yourself.  If you are strict with your Css they can also provide design control 

Here's the `UIColumn` control shown in thr above form:

```csharp
public class UIColumn : UIComponent
{
    [Parameter] public int Columns { get; set; } = 0;

    [Parameter] public int SmallColumns { get; set; } = 0;

    [Parameter] public int MediumColumns { get; set; } = 0;

    [Parameter] public int LargeColumns { get; set; } = 0;

    [Parameter] public int XLargeColumns { get; set; } = 0;

    [Parameter] public int XXLargeColumns { get; set; } = 0;

    [Parameter] public bool AutoDefault { get; set; } = false;

    protected override CSSBuilder CssBuilder => base.CssBuilder
        .AddClass(AutoDefault, "col-auto", "col")
        .AddClass(Columns > 0 && !AutoDefault, $"col-{this.Columns}", $"col-12")
        .AddClass(SmallColumns > 0, $"col-sm-{this.SmallColumns}")
        .AddClass(MediumColumns > 0, $"col-md-{this.MediumColumns}")
        .AddClass(LargeColumns > 0, $"col-lg-{this.LargeColumns}")
        .AddClass(XLargeColumns > 0, $"col-xl-{this.XLargeColumns}")
        .AddClass(XXLargeColumns > 0, $"col-xxl-{this.XXLargeColumns}");
}
```