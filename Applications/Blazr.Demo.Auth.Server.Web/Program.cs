using Blazr.App.Data;
using Blazr.App.UI;
using Blazr.Auth;
using Blazr.Auth.Simple;
using Blazr.Auth.Simple.Core;
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

        // Authentication and Authorization Services
        //{
        //    Services.AddScoped<AuthenticationStateProvider, TestAuthenticationStateProvider>();
        //    Services.AddAuthorization(config =>
        //    {
        //        foreach (var policy in SimpleAuthorizationPolicies.Policies)
        //        {
        //            config.AddPolicy(policy.Key, policy.Value);
        //        }
        //    });
        //}
        Services.AddAppAuthServices();

        Services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(options
            => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}"));
        Services.AddBlazrUIServices();
        Services.AddAppUIServices();
    }
}

var app = builder.Build();

// Add the test data to the InMemory Db
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

app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();
