global using Blazr.App.API;
global using Blazr.App.Infrastructure;
global using Blazr.App.Presentation;
global using Microsoft.EntityFrameworkCore;
using Blazr.App.UI.Vanilla;
using Blazr.RenderState.Server;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.AddBlazrRenderStateServerServices();
builder.Services.AddAppServerMappedInfrastructureServices();
builder.Services.AddAppVanillaPresentationServices();
builder.Services.AddAppVanillaUIServices();

var app = builder.Build();

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

app.MapRazorComponents<Blazr.App.Vanilla.WASM.Server.App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies([typeof(Blazr.App.UI.Vanilla._Imports).Assembly]);

app.Run();
