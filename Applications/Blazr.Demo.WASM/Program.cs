using Blazr.App.Core;
using Blazr.App.Infrastructure;
using Blazr.App.UI;
using Blazr.UI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Blazr.App.UI.AuthApp>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var Services = builder.Services;
{
    Services.AddAppAuthServices();
    //TODO - fix
    //Services.AddWeatherAppWASMDataServices();
    Services.AddBlazrUIServices();
    Services.AddAppUIServices();
}

await builder.Build().RunAsync();
