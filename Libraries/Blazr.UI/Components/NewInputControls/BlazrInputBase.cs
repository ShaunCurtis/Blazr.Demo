/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class BlazrInputBase<TValue> : UIComponentBase, IHandleAfterRender
{
    [CascadingParameter] protected IEditContext editContext { get; set; } = default!;

    [Parameter, EditorRequired] public string FieldName { get; set; } = string.Empty;
    [Parameter, EditorRequired] public Guid FieldObjectUid { get; set; } = Guid.Empty;
    [Parameter] public string? Type { get; set; }
    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue> ValueChanged { get; set; }
    [Parameter] public bool UpdateOnInput { get; set; }
    [Parameter] public bool IsFirstFocus { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; } = new Dictionary<string, object>();

    [DisallowNull] public ElementReference? Element { get; protected set; }

    private bool _hasRendered;

    protected bool NoValidation;

    protected FieldReference Field => FieldReference.Create(FieldObjectUid, FieldName);

    protected string CssClass
        => new CSSBuilder()
        .AddClassFromAttributes(AdditionalAttributes)
        .AddClass(this.editContext is not null && !this.NoValidation, ValidationCss)
        .Build();

    protected string ValidationCss
    {
        get
        {
            var field = FieldReference.Create(this.FieldObjectUid, this.FieldName);
            var isInvalid = this.editContext?.HasMessages(field) ?? false;
            var isChanged = this.editContext?.IsChanged(field) ?? false;

            if (isChanged && isInvalid)
                return "is-invalid";

            if (isChanged && !isInvalid)
                return "is-valid";

            return string.Empty;
        }
    }

    protected string? ValueAsString
        => BlazrInputConverters.GetValueAsString(this.Value, this.Type);

    protected override ValueTask<bool> OnParametersChangedAsync(bool firstRender)
    {
        if (firstRender && editContext is not null)
            this.editContext.ValidationStateUpdated += this.OnValidationStateUpdated;

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

    public Task OnAfterRenderAsync()
    {
        if (_hasRendered.SetTrue() && this.IsFirstFocus && this.Element.HasValue)
            this.Element.Value.FocusAsync();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (editContext is not null)
            this.editContext.ValidationStateUpdated -= this.OnValidationStateUpdated;
    }
}
