global using Blazr.App.Infrastructure;
global using Blazr.App.Presentation;
using Blazr.App.UI.Vanilla;
using Blazr.App.Vanilla.Server;
using Blazr.RenderState.Server;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddBlazrRenderStateServerServices();
builder.Services.AddAppVanillaPresentationServices();
builder.Services.AddAppServerMappedInfrastructureServices();
builder.Services.AddAppVanillaUIServices();

var app = builder.Build();

app.MapDefaultEndpoints();

// get the DbContext factory and add the test data
var factory = app.Services.GetService<IDbContextFactory<InMemoryTestDbContext>>();
if (factory is not null)
    TestDataProvider.Instance().LoadDbContext<InMemoryTestDbContext>(factory);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies([typeof(Blazr.App.UI.Vanilla._Imports).Assembly]);

app.Run();
