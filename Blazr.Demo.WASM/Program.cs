using Blazr.App.Core;
using Blazr.App.Data;
using Blazr.App.UI;
using Blazr.Routing;
using Blazr.UI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Blazr.App.UI.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var Services = builder.Services;
{
    Services.AddBlazrNavigationManager();
    Services.AddAppAuthServices();
    Services.AddWeatherAppWASMDataServices();
    Services.AddBlazrUIServices();
    Services.AddAppUIServices();
}

await builder.Build().RunAsync();
