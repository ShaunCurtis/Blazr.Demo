using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Blazr.RenderState.WASM;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAppClientMappedInfrastructureServices(builder.HostEnvironment.BaseAddress);

builder.Services.AddAppMudBlazorPresentationServices();

builder.AddBlazrRenderStateWASMServices();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
