using CIS;
using CIS.DataAccess;
using CIS.Services;
using Microsoft.EntityFrameworkCore;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new ArgumentException("ConnectionStrings:DefaultConnection must be configured in user secrets or appsettings.json.");

var legacyConnectionString = builder.Configuration
    .GetConnectionString("LegacyConnection");
if (string.IsNullOrWhiteSpace(legacyConnectionString))
    throw new ArgumentException("ConnectionStrings:LegacyConnection must be configured in user secrets or appsettings.json.");

builder.Services.AddDataAccess(opt =>
{
    opt.UseSqlServer(connectionString);
});

builder.Services
    .AddLegacyDatabase(legacyConnectionString);

builder.Services
    .AddRazorComponents();
builder.Services
    .AddRadzenComponents();

builder.Services
    .AddTransient<ImportService>();
builder.Services
    .AddHostedService<FileProcessingBackgroundService>();

var app = builder.Build();

app.InitializeDatabase();

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
    .AddInteractiveServerRenderMode();

app.Run();
