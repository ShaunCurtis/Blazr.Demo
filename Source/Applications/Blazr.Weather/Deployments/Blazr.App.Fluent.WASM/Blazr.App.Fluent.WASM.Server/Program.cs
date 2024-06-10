global using Blazr.App.Infrastructure;
global using Blazr.App.Presentation;
global using Blazr.App.Fluent.Server;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.FluentUI.AspNetCore.Components;
using Blazr.App.Fluent.WASM.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddFluentUIComponents();
builder.Services.AddAppServerMappedInfrastructureServices();
//builder.Services.AddAppServerPresentationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

//app.AddAppAPIEndpoints();

app.MapRazorComponents<Blazr.App.Fluent.WASM.Server.App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies([typeof(Blazr.App.FluentUI._Imports).Assembly]);

app.Run();
