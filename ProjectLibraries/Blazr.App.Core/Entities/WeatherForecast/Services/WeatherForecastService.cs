/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class WeatherForecastService
    : BaseEntityService<WeatherForecastEntity>
{
    private SortedDictionary<Guid, string> _weatherSummaries = new SortedDictionary<Guid, string>();
    private ICQSDataBroker _dataBroker;
    private INotificationService<WeatherSummaryEntity> _weatherSummaryNotificationService;

    public WeatherForecastService(INotificationService<WeatherSummaryEntity> notificationService, ICQSDataBroker cQSDataBroker)
    {
        _weatherSummaryNotificationService = notificationService;
        _dataBroker = cQSDataBroker;
        _weatherSummaryNotificationService.ListUpdated += SummariesListUpdated;
    }

    public async ValueTask<SortedDictionary<Guid, string>> WeatherSummariesAsync()
    {
        if (_weatherSummaries.Count == 0)
            await this.GetWeatherSummariesAsync();

        return _weatherSummaries;
    }

    private async Task GetWeatherSummariesAsync()
    {
        _weatherSummaries.Clear();
        var result = await _dataBroker.ExecuteAsync(new FKListQuery<FkWeatherSummary>());
        if (result.Success)
        {
            foreach (var item in result.Items)
                _weatherSummaries.Add(item.Id, item.Name);
        }
    }

    private async void SummariesListUpdated(object? sender, EventArgs e)
        => await GetWeatherSummariesAsync();

    public void Dispose()
        => _weatherSummaryNotificationService.ListUpdated -= SummariesListUpdated;
}
