﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Presentation.Toasts;

namespace Blazr.App.Presentation;

public class WeatherForecastEditPresenter

{
    private readonly IDataBroker _dataBroker;
    private readonly IAppToastService _toastService;

    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public EditContext? EditContext { get; private set; }
    public WeatherForecastEditContext RecordEditContext { get; private set; }
    public bool IsNew { get; private set; }

    public bool IsInvalid => this.EditContext?.GetValidationMessages().Any() ?? false;

    public WeatherForecastEditPresenter(IDataBroker dataBroker, IAppToastService toastService)
    {
        _dataBroker = dataBroker;
        this.RecordEditContext = new(new());
        //this.EditContext = new(this.RecordEditContext);
        _toastService = toastService;
    }

    public async Task LoadAsync(WeatherForecastId id)
    {
        this.LastDataResult = DataResult.Success();
        this.IsNew = false;

        // The Update Path.  Get the requested record if it exists
        if (id.Value != Guid.Empty)
        {
            var request = ItemQueryRequest<WeatherForecastId>.Create(id);
            var result = await _dataBroker.ExecuteQueryAsync<DmoWeatherForecast, WeatherForecastId>(request);
            LastDataResult = result;
            if (this.LastDataResult.Successful)
            {
                RecordEditContext = new(result.Item!);
                this.EditContext = new(this.RecordEditContext);
                //this.EditContext.Properties["ComponentId"] = Guid.NewGuid();
                //this.EditContext.OnFieldChanged += this.OnChange;
            }
            return;
        }

        // The new path.  Get a new record
        this.RecordEditContext = new(new() { WeatherForecastId = new(Guid.NewGuid()), Date = DateOnly.FromDateTime(DateTime.Now), Summary = "Not Provided" });
        this.EditContext = new(this.RecordEditContext);
        //this.EditContext.Properties["ComponentId"] = Guid.NewGuid();
        //this.EditContext.OnFieldChanged += this.OnChange;
        this.IsNew = true;
    }

    // TODO -  Debug Code
    //private void OnChange(object? sender, FieldChangedEventArgs e)
    //{
    //    var x = this.EditContext!.GetValidationMessages().Count();
    //    if (this.EditContext!.Properties.TryGetValue("ComponentId", out object? value))
    //        Console.WriteLine($"{value?.ToString()} changed");

    //    Console.WriteLine($"{e.FieldIdentifier.FieldName} changed");
    //}

    public async Task<IDataResult> SaveItemAsync()
    {

        if (!this.RecordEditContext.IsDirty)
        {
            this.LastDataResult = DataResult.Failure("The record has not changed and therefore has not been updated.");
            _toastService.ShowWarning("The record has not changed and therefore has not been updated.");
            return this.LastDataResult;
        }

        var record = RecordEditContext.AsRecord;
        var command = new CommandRequest<DmoWeatherForecast>(record, this.IsNew ? CommandState.Add : CommandState.Update);
        var result = await _dataBroker.ExecuteCommandAsync<DmoWeatherForecast>(command);

        if (result.Successful)
        {
            var outcome = this.IsNew ? "saved" : "updated";
            _toastService.ShowSuccess($"The Weather Forecast was {outcome}.");
        }
        else
            _toastService.ShowError(result.Message ?? "The Weather Forecast could not be saved.");

        this.LastDataResult = result;
        return result;
    }
}
