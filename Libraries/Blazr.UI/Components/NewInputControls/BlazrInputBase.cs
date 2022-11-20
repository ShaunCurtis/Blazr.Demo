/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class BlazrInputBase<TValue> : UIComponentBase
{
    [CascadingParameter] public IEditContext RecordEditContext { get; set; } = default!;

    [Parameter, EditorRequired] public string? FieldName { get; set; }
    [Parameter] public string? Type { get; set; }
    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue> ValueChanged { get; set; }
    [Parameter] public bool UpdateOnInput { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; } = new Dictionary<string, object>();

    protected bool NoValidation;

    protected string CssClass
        => new CSSBuilder()
        .AddClassFromAttributes(AdditionalAttributes)
        .AddClass(this.RecordEditContext is not null && !this.NoValidation, ValidationCss)
        .Build();

    protected string ValidationCss
        => this.RecordEditContext?.HasMessages(this.FieldName) ?? false
        ? "is-invalid"
        : "is-valid";

    protected string? ValueAsString
        => GetValueAsString(this.Value, this.Type);

    protected override ValueTask<bool> OnParametersChangedAsync(bool firstRender)
    {
        if (firstRender && RecordEditContext is not null)
            this.RecordEditContext.ValidationStateUpdated += this.OnValidationStateUpdated;

        return ValueTask.FromResult(true);
    }

    protected void OnChanged(ChangeEventArgs e)
    {
        if (BindConverter.TryConvertTo<TValue>(e.Value, System.Globalization.CultureInfo.InvariantCulture, out TValue? result))
            this.ValueChanged.InvokeAsync(result);
    }

    protected void OnValidationStateUpdated(object? sender, ValidationStateEventArgs e)
        => this.StateHasChanged();

    protected void OnFieldChanged(object? sender, string? field)
       => this.StateHasChanged();

    public void Dispose()
    {
        if (RecordEditContext is not null)
            this.RecordEditContext.ValidationStateUpdated -= this.OnValidationStateUpdated;
    }

    private static string? GetValueAsString(TValue? initialValue, string? type) =>
        initialValue switch
        {
            string @value => value,
            int @value => BindConverter.FormatValue(value),
            long @value => BindConverter.FormatValue(value),
            short @value => BindConverter.FormatValue(value),
            float @value => BindConverter.FormatValue(value),
            double @value => BindConverter.FormatValue(value),
            decimal @value => BindConverter.FormatValue(value),
            TimeOnly @value => BindConverter.FormatValue(value, format: "HH:mm:ss"),
            DateOnly @value => BindConverter.FormatValue(value, format: "yyyy-MM-dd"),
            DateTime @value => GetDateString(value, type),
            DateTimeOffset @value => GetDateString(value, type),
            _ => initialValue?.ToString() ?? throw new InvalidOperationException($"Unsupported type {initialValue?.GetType()}")
        };

    private static string GetDateString(DateTime value, string? type)
        => type?.ToLower() switch
        {
            "date" => BindConverter.FormatValue(value, format: "yyyy-MM-dd"),
            "time" => BindConverter.FormatValue(value, format: "HH:mm:ss"),
            _ => BindConverter.FormatValue(value, format: "yyyy-MM-ddTHH:mm:ss")
        };

    private static string GetDateString(DateTimeOffset value, string? type)
        => type?.ToLower() switch
        {
            "date" => BindConverter.FormatValue(value, format: "yyyy-MM-dd"),
            "time" => BindConverter.FormatValue(value, format: "HH:mm:ss"),
            _ => BindConverter.FormatValue(value, format: "yyyy-MM-ddTHH:mm:ss")
        };
}
