using Blazr.App.Core;
using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Blazr.OneWayStreet.Core;
using Blazr.OneWayStreet.Infrastructure;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient();
builder.Services.AddHttpClient(AppDictionary.WeatherForecast.WeatherHttpClient, client => { client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); });

builder.Services.AddScoped<IListRequestHandler, ListRequestAPIHandler>();

builder.Services.AddScoped<IListRequestHandler<DmoWeatherForecast>, WeatherAPIListRequestHandler>();
builder.Services.AddTransient<IListPresenter<DmoWeatherForecast>, ListPresenter<DmoWeatherForecast>>();

builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
