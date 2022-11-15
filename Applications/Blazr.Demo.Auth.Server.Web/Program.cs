using Blazr.App.Core;
using Blazr.App.Infrastructure;
using Blazr.App.UI;
using Blazr.Routing;
using Blazr.UI;
using Microsoft.AspNetCore.Components.Authorization;
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

        Services.AddAuthentication();
        Services.AddAuthorization();

        Services.AddAppAuthServices();

        Services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(options
            => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}"));

        Services.AddBlazrUIServices();
        Services.AddAppUIServices();
    }
}

var app = builder.Build();

// Add the test data to the In Memory Db
WeatherAppDataServices.AddTestData(app.Services);

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

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();
