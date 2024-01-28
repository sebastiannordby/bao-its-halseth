using CIS.Client.Layout;
using CIS.Components;
using CIS.Components.Layout;
using MudBlazor;
using MudBlazor.Services;
using CIS;
using CIS.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddMudServices();

var connectionString = builder.Configuration["ConnectionString"];
if (string.IsNullOrWhiteSpace(connectionString))
    throw new ArgumentException("ConnectionString must be configured in user secrets or appsettings.json.");

builder.Services.AddDataAccess(connectionString);

var app = builder.Build();

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
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MainLayout).Assembly);

app.Run();
