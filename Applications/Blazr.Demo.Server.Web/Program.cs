global using Blazr.Demo.Core;

using Blazr.Demo.Data;
using Blazr.Core;
using Blazr.Core.Toaster;
using Blazr.Routing;
using Blazr.UI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{
    var Services = builder.Services;
    {
        Services.AddRazorPages();
        Services.AddServerSideBlazor();
        Services.AddControllersWithViews();

        Services.AddBlazrNavigationManager();
        Services.AddSingleton<ToasterService>();
        Services.AddSingleton<ModalService>();

        Services.AddWeatherAppServerDataServices();

        Services.AddWeatherForecastServices();
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();
