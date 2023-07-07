using Blazr.App.Core.Auth;
using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Blazr.App.UI;
using Blazr.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAppServerInfrastructureServices();
builder.Services.AddAppPresentationServices();
builder.Services.AddAppUIServices();
builder.Services.AddAppServerAuthServices();
var app = builder.Build();

// get the DbContext factory and add the test data
var factory = app.Services.GetService<IDbContextFactory<InMemoryInvoiceDbContext>>();
if (factory is not null)
    InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceDbContext>(factory);

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
app.MapFallbackToPage("/_Host");

app.Run();
