/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.SPA.Components
{
    public partial class InputSelectDataList<TValue> : InputBase<TValue>
    {
        private ValidationMessageStore? _parsingValidationMessages;
        private bool _previousParsingAttemptFailed = false;
        private string _selectedValue = string.Empty;
        private TValue? _selectedKey = default;

        [Parameter] public SortedDictionary<TValue, string> DataList { get; set; } = new SortedDictionary<TValue, string>();

        private string dataListId { get; set; } = Guid.NewGuid().ToString();

        private string ClearCss => new CSSBuilder("input-group-text")
            .AddClass("is-valid", "is-invalid", this.IsValid())
            .Build();

        protected override void OnInitialized()
        {
            if (DataList is null)
                throw new InvalidOperationException($"No Data List Found!");
            if (DataList.ContainsKey(this.Value))
            {
                _selectedValue = DataList[this.Value];
                _selectedKey = this.Value;
            }
        }

        protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
        {
            result = default(TValue);
            validationErrorMessage = string.Empty;
            return true;
        }

        protected async Task OnValueChanged(ChangeEventArgs e)
        {
            await this.SetValue(e.Value.ToString());
        }

        private async Task SetValue(string value)
        {
            // Check if we have a ValidationMessageStore
            // Either get one or clear the existing one
            if (_parsingValidationMessages == null)
                _parsingValidationMessages = new ValidationMessageStore(EditContext);
            else
                _parsingValidationMessages?.Clear(FieldIdentifier);

            // Check if we have a match and set it if we do
            if (GetDictionaryMatch(value, out KeyValuePair<TValue, string> match))
            {
                this.Value = match.Key;
                await this.ValueChanged.InvokeAsync(match.Key);
                _selectedValue = match.Value;
                _selectedKey = match.Key;
                EditContext.NotifyFieldChanged(this.FieldIdentifier);
                // Check if the last entry failed validation.  If so notify the EditContext that validation has changed i.e. it's now clear
                if (_previousParsingAttemptFailed)
                {
                    EditContext.NotifyValidationStateChanged();
                    _previousParsingAttemptFailed = false;
                }
            }
            // We're reverting to the last entry if we have one.  If we don't then we generate a validation message
            else
            {
                if (DataList.ContainsKey(_selectedKey))
                {
                    _selectedValue = string.Empty;
                    await Task.Yield();
                    _selectedValue = DataList[_selectedKey];
                    this.Value = _selectedKey;
                    await this.ValueChanged.InvokeAsync(_selectedKey);
                }
                else
                {
                    // No match so add a message to the message store
                    _parsingValidationMessages?.Add(FieldIdentifier, "You must choose a valid selection");
                    // keep track of validation state for the next iteration
                    _previousParsingAttemptFailed = true;
                    // notify the EditContext whick will precipitate a Validation Message general update
                    this.EditContext.NotifyValidationStateChanged();
                    this.Value = default;
                    this._selectedValue = string.Empty;
                    EditContext.NotifyFieldChanged(this.FieldIdentifier);
                }
            }
        }

        private bool GetDictionaryMatch(string value, out KeyValuePair<TValue, string> match)
        {
            match = new KeyValuePair<TValue, string>(default, default);

            // Check if we have a match and set it if we do
            var haveValue = DataList.ContainsValue(value);
            if (haveValue)
                match = DataList.First(item => item.Value.Contains(value));
            if (!haveValue)
            {
                var matches = DataList.Where(item => item.Value.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (matches is not null && matches.Count() > 0)
                {
                    match = matches[0];
                    haveValue = true;
                }
            }
            return haveValue;
        }


        protected void ClearValue()
        {
            _selectedValue = string.Empty;
            this.Value = default;
            this.ValueChanged.InvokeAsync(default);
        }

        protected bool IsValid()
        {
            var validationMessages = this.EditContext.GetValidationMessages(this.FieldIdentifier);
            return validationMessages is null || validationMessages.Count() == 0;
        }
    }
}
