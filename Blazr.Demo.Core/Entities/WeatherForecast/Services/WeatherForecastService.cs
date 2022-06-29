/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public class WeatherForecastService
    : IEntityService
{
    private SortedDictionary<Guid, string> _weatherSummaries = new SortedDictionary<Guid, string>();
    private IQueryDataBroker _queryBroker;
  private ICQSDataBroker _cqsDataBroker;
    private INotificationService<WeatherSummaryService> _weatherSummaryNotificationService;

    public WeatherForecastService(IQueryDataBroker queryDataBroker, INotificationService<WeatherSummaryService> notificationService, ICQSDataBroker cQSDataBroker)
    { 
        _queryBroker = queryDataBroker;
        _weatherSummaryNotificationService = notificationService;
        _cqsDataBroker = cQSDataBroker;
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
        var result = await _queryBroker.ExecuteAsync(new FKListQuery<FkWeatherSummaryId>());
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
