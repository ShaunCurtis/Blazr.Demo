using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Blazr.RenderState.WASM;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAppClientMappedInfrastructureServices(builder.HostEnvironment.BaseAddress);

builder.Services.AddAppFluentUIPresentationServices();

builder.AddBlazrRenderStateWASMServices();

builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
