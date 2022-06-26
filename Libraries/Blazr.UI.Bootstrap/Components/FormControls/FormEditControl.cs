/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Bootstrap;

public class FormEditControl<TValue> : ComponentBase
{
    [Parameter]
    public TValue? Value { get; set; }

    [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter] public Expression<Func<TValue>>? ValueExpression { get; set; }

    [Parameter] public string? Label { get; set; }

    [Parameter] public string? HelperText { get; set; }

    [Parameter] public string DivCssClass { get; set; } = "mb-2";

    [Parameter] public string LabelCssClass { get; set; } = "form-label small text-muted";

    [Parameter] public string ControlCssClass { get; set; } = "form-control";

    [Parameter] public Type? ControlType { get; set; }

    [Parameter] public bool ShowValidation { get; set; }

    [Parameter] public bool ShowLabel { get; set; } = true;

    [Parameter] public bool IsRequired { get; set; }

    [Parameter] public bool IsRow { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }

    [CascadingParameter] EditContext CurrentEditContext { get; set; } = default!;


    private readonly string formId = Guid.NewGuid().ToString();

    private bool IsLabel => this.ShowLabel && (!string.IsNullOrWhiteSpace(this.Label) || !string.IsNullOrWhiteSpace(this.FieldName));

    private bool IsValid;

    private FieldIdentifier _fieldIdentifier;

    private ValidationMessageStore? _messageStore;

    private string? DisplayLabel => this.Label ?? this.FieldName;

    private string? FieldName
    {
        get
        {
            string? fieldName = null;
            if (this.ValueExpression != null)
                ParseAccessor(this.ValueExpression, out var model, out fieldName);
            return fieldName;
        }
    }

    private string MessageCss => CSSBuilder.Class()
        .AddClass(this.IsValid, "valid-feedback", "invalid-feedback")
        .Build();

    private string ControlCss => CSSBuilder.Class(this.ControlCssClass)
        .AddClass(this.IsValid, "is-valid", "is-invalid")
        .Build();

    protected override void OnInitialized()
    {
        if (CurrentEditContext is null)
            throw new InvalidOperationException($"No Cascading Edit Context Found!");

        if (ValueExpression is null)
            throw new InvalidOperationException($"No ValueExpression defined for the Control!  Define a Bind-Value.");

        if (!ValueChanged.HasDelegate)
            throw new InvalidOperationException($"No ValueChanged defined for the Control! Define a Bind-Value.");

        CurrentEditContext.OnFieldChanged += FieldChanged;
        CurrentEditContext.OnValidationStateChanged += ValidationStateChanged;
        _messageStore = new ValidationMessageStore(this.CurrentEditContext);
        _fieldIdentifier = FieldIdentifier.Create(ValueExpression);
        if (_messageStore is null)
            throw new InvalidOperationException($"Cannot set the Validation Message Store!");

        var messages = CurrentEditContext.GetValidationMessages(_fieldIdentifier).ToList();
        var showHelpText = (messages.Count == 0) && this.IsRequired && this.Value is null;
        if (showHelpText && !string.IsNullOrWhiteSpace(this.HelperText))
            _messageStore.Add(_fieldIdentifier, this.HelperText);
    }

    protected void ValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        var messages = CurrentEditContext.GetValidationMessages(_fieldIdentifier).ToList();
        if (messages != null && messages.Count > 1)
        {
            _messageStore?.Clear();
        }
    }

    protected void FieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (e.FieldIdentifier.Equals(_fieldIdentifier))
            _messageStore?.Clear();
    }

    protected override void OnParametersSet()
    {
        this.IsValid = true;
        {
            if (this.IsRequired)
            {
                this.IsValid = false;
                var messages = CurrentEditContext.GetValidationMessages(_fieldIdentifier).ToList();
                if (messages is null || messages.Count == 0)
                    this.IsValid = true;
            }
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (IsRow)
            builder.AddContent(1, RowFragment);
        else
            builder.AddContent(2, BaseFragment);
    }

    private RenderFragment BaseFragment => (builder) =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(10, "class", this.DivCssClass);
        builder.AddContent(40, this.LabelFragment);
        builder.AddContent(60, this.ControlFragment);
        builder.AddContent(70, this.ValidationFragment);
        builder.CloseElement();
    };

    private RenderFragment RowFragment => (builder) =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(10, "class", "row form-group");
        builder.OpenElement(20, "div");
        builder.AddAttribute(30, "class", "col-12 col-md-3");
        builder.AddContent(40, this.LabelFragment);
        builder.CloseElement();
        builder.OpenElement(40, "div");
        builder.AddAttribute(50, "class", "col-12 col-md-9");
        builder.AddContent(60, this.ControlFragment);
        builder.AddContent(70, this.ValidationFragment);
        builder.CloseElement();
        builder.CloseElement();
    };

    private RenderFragment LabelFragment => (builder) =>
    {
        if (this.IsLabel)
        {
            builder.OpenElement(110, "label");
            builder.AddAttribute(120, "for", this.formId);
            builder.AddAttribute(130, "class", this.LabelCssClass);
            builder.AddContent(140, this.DisplayLabel);
            builder.CloseElement();
        }
    };

    private RenderFragment ControlFragment => (builder) =>
    {
        if (this.ControlType is not null)
        {
            builder.OpenComponent(210, this.ControlType);
            builder.AddAttribute(220, "class", this.ControlCss);
            builder.AddAttribute(230, "Value", this.Value);
            builder.AddAttribute(240, "ValueChanged", EventCallback.Factory.Create(this, this.ValueChanged));
            builder.AddAttribute(250, "ValueExpression", this.ValueExpression);
            if (this.ChildContent is not null)
                builder.AddAttribute(260, "ChildContent", this.ChildContent);

            builder.CloseComponent();
        }
        else if (this.ChildContent is not null)
            builder.AddContent(200, this.ChildContent);
    };

    private RenderFragment ValidationFragment => (builder) =>
    {
        if (this.ShowValidation && !this.IsValid)
        {
            builder.OpenElement(310, "div");
            builder.AddAttribute(320, "class", MessageCss);
            builder.OpenComponent<ValidationMessage<TValue>>(330);
            builder.AddAttribute(340, "For", this.ValueExpression);
            builder.CloseComponent();
            builder.CloseElement();
        }
        else if (!string.IsNullOrWhiteSpace(this.HelperText))
        {
            builder.OpenElement(350, "div");
            builder.AddAttribute(360, "class", MessageCss);
            builder.AddContent(370, this.HelperText);
            builder.CloseElement();
        }
    };

    // Code lifted from FieldIdentifier.cs
    private static void ParseAccessor<T>(Expression<Func<T>> accessor, out object model, out string fieldName)
    {
        var accessorBody = accessor.Body;
        if (accessorBody is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert && unaryExpression.Type == typeof(object))
            accessorBody = unaryExpression.Operand;

        if (!(accessorBody is MemberExpression memberExpression))
            throw new ArgumentException($"The provided expression contains a {accessorBody.GetType().Name} which is not supported. {nameof(FieldIdentifier)} only supports simple member accessors (fields, properties) of an object.");

        fieldName = memberExpression.Member.Name;
        if (memberExpression.Expression is ConstantExpression constantExpression)
        {
            if (constantExpression.Value is null)
                throw new ArgumentException("The provided expression must evaluate to a non-null value.");
            model = constantExpression.Value;
        }
        else if (memberExpression.Expression != null)
        {
            var modelLambda = Expression.Lambda(memberExpression.Expression);
            var modelLambdaCompiled = (Func<object?>)modelLambda.Compile();
            var result = modelLambdaCompiled();
            if (result is null)
                throw new ArgumentException("The provided expression must evaluate to a non-null value.");
            model = result;
        }
        else
            throw new ArgumentException($"The provided expression contains a {accessorBody.GetType().Name} which is not supported. {nameof(FieldIdentifier)} only supports simple member accessors (fields, properties) of an object.");
    }

}


