namespace Blazr.SPA.Components;

public partial class InputDataList : InputBase<string>
{
    private ValidationMessageStore? _parsingValidationMessages;
    private bool _previousParsingAttemptFailed = false;
    private string? _previousValue = string.Empty;

    [Parameter] public List<string>? DataList { get; set; }

    private List<string> _dataList
    {
        get
        {
            if (DataList is null)
                throw new InvalidOperationException($"No Data List Found!");
            return this.DataList!;
        }
    }

    [Parameter] public bool RestrictToList { get; set; }

    private string dataListId { get; set; } = Guid.NewGuid().ToString();

    private string ClearCss => new CSSBuilder("input-group-text")
        .AddClass("is-valid", "is-invalid", this.IsValid())
        .Build();

    protected override void OnInitialized()
    {
        if (DataList is null)
            throw new InvalidOperationException($"No Data List Found!");
        _previousValue = this.Value;
    }

    protected override bool TryParseValueFromString(string? value, out string result, out string validationErrorMessage)
    {
        result = value ?? String.Empty;
        validationErrorMessage = string.Empty;
        return true;
    }

    protected async Task OnValueChanged(ChangeEventArgs e)
    {
        await this.SetValue(e.Value?.ToString() ?? String.Empty);
    }

    private async Task SetValue(string value)
    {
        // Check if we have a ValidationMessageStore
        // Either get one or clear the existing one
        if (_parsingValidationMessages == null)
            _parsingValidationMessages = new ValidationMessageStore(EditContext);
        else
            _parsingValidationMessages?.Clear(FieldIdentifier);

        // Set defaults
        string val = string.Empty;
        var haveValue = false;

        // Check if we're in restricted mode
        if (this.RestrictToList)
        {
            // Check if we have a match and set it if we do
            haveValue = this.GetMatch(value, out val);
        }
        else
        {
            haveValue = true;
            val = value;
        }

        // check if we have a valid value
        if (haveValue)
        {
            await this.ValueChanged.InvokeAsync(val);
            this.Value = val;
            EditContext.NotifyFieldChanged(this.FieldIdentifier);
            // Check if the last entry failed validation.  If so notify the EditContext that validation has changed i.e. it's now clear
            if (_previousParsingAttemptFailed)
            {
                EditContext.NotifyValidationStateChanged();
                _previousParsingAttemptFailed = false;
                await ValueChanged.InvokeAsync(string.Empty);
            }
        }
        // We don't have a valid value
        else
        {
            // check if we're reverting to the last entry.  If we don't have one the generate error message
            if (_previousValue is not null && _dataList.Contains(_previousValue))
            {
                this.Value = string.Empty;
                await Task.Yield();
                this.Value = _previousValue;
                await this.ValueChanged.InvokeAsync(_previousValue);
            }
            else
            {
                // No match so add a message to the message store
                _parsingValidationMessages?.Add(FieldIdentifier, "You must choose a valid selection");
                // keep track of validation state for the next iteration
                _previousParsingAttemptFailed = true;
                // notify the EditContext whick will precipitate a Validation Message general update
                EditContext.NotifyFieldChanged(this.FieldIdentifier);
                EditContext.NotifyValidationStateChanged();
                this.Value = string.Empty;
                EditContext.NotifyFieldChanged(this.FieldIdentifier);
            }
        }
    }
    private bool GetMatch(string value, out string match)
    {
        match = string.Empty;

        // Check if we have a match and set it if we do
        var haveValue = _dataList.Contains(value);
        if (haveValue)
            match = _dataList.First(item => item.Contains(value));
        if (!haveValue)
        {
            var matches = _dataList.Where(item => item.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (matches is not null && matches.Count() > 0)
            {
                match = matches[0];
                haveValue = true;
            }
        }
        return haveValue;
    }


    protected async Task ClearValue()
    {
        _previousValue = this.Value;
        Value = string.Empty;
        await ValueChanged.InvokeAsync(string.Empty);
    }
    protected bool IsValid()
    {
        var validationMessages = this.EditContext.GetValidationMessages(this.FieldIdentifier);
        return validationMessages is null || validationMessages.Count() == 0;
    }

}

