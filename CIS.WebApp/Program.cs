using CIS.Application;
using CIS.WebApp.Components;
using CIS.WebApp.Components.Account;
using CIS.Application;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CIS.WebApp.Services;
using CIS.WebApp.Extensions;
using Radzen;
using CIS.Application.Shopify.Options;
using CIS.Application.Shopify;
using ShopifySharp.Extensions.DependencyInjection;
using ShopifySharp;
using CIS.Application.Hubs;

namespace CIS.WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("ConnectionStrings:DefaultConnection must be configured in user secrets or appsettings.json.");

            var legacyConnectionString = builder.Configuration
                .GetConnectionString("LegacyConnection");
            if (string.IsNullOrWhiteSpace(legacyConnectionString))
                throw new ArgumentException("ConnectionStrings:LegacyConnection must be configured in user secrets or appsettings.json.");

            builder.Services.AddShopifyFeature();
            builder.Services.Configure<ShopifyClientServiceOptions>(
                builder.Configuration.GetSection("Shopify"));

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services
                .AddRadzenComponents();

            builder.Services.AddSignalR();

            builder.Services.AddShopifySharp<LeakyBucketExecutionPolicy>();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(UserTypes.ADMINISTRATOR,
                     policy => policy.RequireRole(UserTypes.ADMINISTRATOR));
                options.AddPolicy(UserTypes.CUSTOMER,
                     policy => policy.RequireRole(UserTypes.CUSTOMER));
            });

            builder.Services.AddCISApplication(connectionString);
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddHttpContextAccessor();

            builder.Services
                .AddSWNDistroLegacyDatabase(legacyConnectionString);

            builder.Services
                .AddSingleton<ImportLegacyDataBackgroundService>()
                .AddSingleton<ImportLegacyDataHub>()
                .AddScoped<ImportService>();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<CISDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            var app = builder.Build();

            app.MapHub<ImportLegacyDataHub>("/import-legacy-hub");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            await app.InitializeDatabase(
                requiresMigrationFromLegacy: true,
                insertTestUser: true);
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
    }
}
