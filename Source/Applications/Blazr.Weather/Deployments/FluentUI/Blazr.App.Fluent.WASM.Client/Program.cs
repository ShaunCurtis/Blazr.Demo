using Blazr.App.Core;
using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Blazr.RenderState.WASM;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient();
builder.Services.AddHttpClient(AppDictionary.WeatherForecast.WeatherHttpClient, client => { client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); });

builder.Services.AddAppClientMappedInfrastructureServices();

builder.Services.AddAppFluentUIPresentationServices();

builder.AddBlazrRenderStateWASMServices();

builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
