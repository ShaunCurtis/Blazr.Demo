global using Blazr.App.API;
global using Blazr.App.Infrastructure;
global using Blazr.App.Presentation.Mud;
using Blazr.App.UI.Mud;
using Blazr.RenderState.Server;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddMudServices();
builder.AddBlazrRenderStateServerServices();
builder.Services.AddAppServerMappedInfrastructureServices();
builder.Services.AddAppMudBlazorPresentationServices();
builder.Services.AddAppMudUIServices();

var app = builder.Build();

// Aspire endpoints
app.MapDefaultEndpoints();

// Adds in all the API endpoints
app.AddAppAPIEndpoints();

// get the DbContext factory and add the test data
var factory = app.Services.GetService<IDbContextFactory<InMemoryTestDbContext>>();
if (factory is not null)
    TestDataProvider.Instance().LoadDbContext<InMemoryTestDbContext>(factory);

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

app.MapRazorComponents<Blazr.App.MudBlazor.WASM.Server.App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies([typeof(Blazr.App.UI.Mud._Imports).Assembly]);

app.Run();
