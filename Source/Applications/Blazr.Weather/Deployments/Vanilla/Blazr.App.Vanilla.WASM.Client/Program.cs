using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Blazr.App.UI.Vanilla;
using Blazr.RenderState.WASM;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAppClientMappedInfrastructureServices(builder.HostEnvironment.BaseAddress);

builder.Services.AddAppVanillaPresentationServices();

builder.Services.AddAppVanillaUIServices();

builder.AddBlazrRenderStateWASMServices();

await builder.Build().RunAsync();
