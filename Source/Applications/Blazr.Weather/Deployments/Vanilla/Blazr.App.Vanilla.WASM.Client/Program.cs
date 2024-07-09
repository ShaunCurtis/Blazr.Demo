using Blazr.App.Core;
using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Blazr.App.Presentation.Vanilla;
using Blazr.OneWayStreet.Core;
using Blazr.OneWayStreet.Infrastructure;
using Blazr.RenderState.WASM;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient();
builder.Services.AddHttpClient(AppDictionary.WeatherForecast.WeatherHttpClient, client => { client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); });

builder.Services.AddScoped<IDataBroker, DataBroker>();

builder.Services.AddScoped<IListRequestHandler, ListRequestAPIHandler>();
builder.Services.AddScoped<IItemRequestHandler, ItemRequestAPIHandler>();
builder.Services.AddScoped<ICommandHandler, CommandAPIHandler>();

builder.Services.AddScoped<IListRequestHandler<DmoWeatherForecast>, WeatherForecastAPIListRequestHandler>();
builder.Services.AddScoped<IItemRequestHandler<DmoWeatherForecast, WeatherForecastId>, WeatherForecastAPIItemRequestHandler>();
builder.Services.AddScoped<ICommandHandler<DmoWeatherForecast>, WeatherForecastAPICommandHandler>();

builder.Services.AddAppVanillaPresentationServices();

builder.AddBlazrRenderStateWASMServices();

await builder.Build().RunAsync();
