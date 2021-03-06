/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public class EditViewTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public EditViewTests()
    {
        _weatherTestDataProvider = WeatherTestDataProvider.Instance();
    }

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        Action<DbContextOptionsBuilder> dbOptions = options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}");
        services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(dbOptions);
        services.AddWeatherForecastServices();
        var provider = services.BuildServiceProvider();

        WeatherAppDataServices.AddTestData(provider);

        return provider;
    }

    [Fact]
    public async void TestGetRecordDataBroker()
    {
        var services = GetServiceProvider();

        var view = services.GetService<IEditService<DboWeatherForecast, DeoWeatherForecast>>()!;
        var notificationService = services.GetService<INotificationService<WeatherForecastService>>();

        var testRecord = _weatherTestDataProvider.GetRandomRecord()!;
        var id = testRecord.WeatherForecastId;

        await view.LoadRecordAsync(id);

        Assert.Equal(view.Record, testRecord);
    }

    [Fact]
    public async void TestUpdateRecordDataBroker()
    {
        var services = GetServiceProvider();

        var view = services.GetService<IEditService<DboWeatherForecast, DeoWeatherForecast>>()!;
        var notificationService = services.GetService<INotificationService<WeatherForecastService>>();

        var listUpdated = false;
        var listPaged = false;
        RecordEventArgs? recordEvent = null;
        
        notificationService!.ListUpdated += delegate (object? sender, EventArgs e)
        { listUpdated = true; };

        notificationService!.ListPaged += delegate (object? sender, PagingEventArgs e)
        { listPaged = true; };

        notificationService!.RecordChanged += delegate (object? sender, RecordEventArgs e)
        { recordEvent = e; };

        var baseRecord = _weatherTestDataProvider.GetRandomRecord()!;
        var id = baseRecord.WeatherForecastId;

        var editedRecord = baseRecord with { TemperatureC = baseRecord.TemperatureC + 10 };

        // get the record into the view
        await view.LoadRecordAsync(id);

        // change a value in the edit model
        view.EditModel.TemperatureC = view.EditModel.TemperatureC + 10;

        Assert.True(view.EditModel.IsDirty);
        Assert.Equal(editedRecord, view.EditModel.Record);

        // Update the record
        await view.UpdateRecordAsync();

        Assert.Equal(view.Record, editedRecord);
        Assert.True(listUpdated);
        Assert.False(listPaged);
        Assert.Equal(id, recordEvent!.RecordId);
    }

    [Fact]
    public async void TestAddRecordDataBroker()
    {
        var services = GetServiceProvider();

        var view = services.GetService<IEditService<DboWeatherForecast, DeoWeatherForecast>>()!;
        var notificationService = services.GetService<INotificationService<WeatherForecastService>>();

        var listUpdated = false;
        var listPaged = false;
        RecordEventArgs? recordEvent = null;

        notificationService!.ListUpdated += delegate (object? sender, EventArgs e)
        { listUpdated = true; };

        notificationService!.ListPaged += delegate (object? sender, PagingEventArgs e)
        { listPaged = true; };

        notificationService!.RecordChanged += delegate (object? sender, RecordEventArgs e)
        { recordEvent = e; };

        var baseRecord = _weatherTestDataProvider.GetForecast()! with { WeatherForecastId = Guid.Empty};

        // get the record into the view
        await view.GetNewRecordAsync(null);

        // change a value in the edit model
        view.EditModel.TemperatureC = baseRecord.TemperatureC;
        view.EditModel.Date = baseRecord.Date;
        view.EditModel.SummaryId = baseRecord.WeatherSummaryId;

        var newRecord = baseRecord with { WeatherForecastId = view.EditModel.AsNewRecord.WeatherForecastId };
        var id = newRecord.WeatherForecastId;

        Assert.True(view.EditModel.IsDirty);
        Assert.Equal(baseRecord, view.EditModel.Record);
        Assert.Equal(newRecord, view.EditModel.AsNewRecord);

        // Update the record
        await view.AddRecordAsync();

        Assert.Equal(view.Record, newRecord);
        Assert.True(listUpdated);
        Assert.False(listPaged);
        Assert.Equal(id, recordEvent!.RecordId);
    }

    [Fact]
    public async void TestDeleteRecordDataBroker()
    {
        var services = GetServiceProvider();

        var view = services.GetService<IEditService<DboWeatherForecast, DeoWeatherForecast>>()!;
        var notificationService = services.GetService<INotificationService<WeatherForecastService>>();

        var listUpdated = false;
        var listPaged = false;
        RecordEventArgs? recordEvent = null;

        notificationService!.ListUpdated += delegate (object? sender, EventArgs e)
        { listUpdated = true; };

        notificationService!.ListPaged += delegate (object? sender, PagingEventArgs e)
        { listPaged = true; };

        notificationService!.RecordChanged += delegate (object? sender, RecordEventArgs e)
        { recordEvent = e; };

        var baseRecord = _weatherTestDataProvider.GetRandomRecord()!;
        var id = baseRecord.WeatherForecastId;

        // get the record into the view
        await view.LoadRecordAsync(id);

        // Update the record
        await view.DeleteRecordAsync();

        Assert.Null(view.Record);

        // get the record into the view
        await view.LoadRecordAsync(id);

        Assert.Null(view.Record);

        Assert.True(listUpdated);
        Assert.False(listPaged);
        Assert.Equal(id, recordEvent!.RecordId);
    }

}
