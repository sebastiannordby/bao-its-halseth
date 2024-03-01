using CIS.Application.Legacy;
using CIS.Application.Shared.Infrastructure;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using CIS.Application.Shopify;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CIS.Application;
using Radzen;
using CIS.Application.Shopify.Options;
using ShopifySharp.Extensions.DependencyInjection;
using ShopifySharp;
using CIS.Application.Hubs;
using CIS.Application.Features.Stores;
using CIS.Application.Features.Orders;
using CIS.Application.Features.Products;

namespace CIS.Application
{
    public static class CISApplicationInstaller
    {
        public static IServiceCollection AddCISApplication(
            this IServiceCollection services, string connectionString)
        {
            services.AddDbContextPool<CISDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services
                .AddScoped<IMigrationTaskRepo, MigrationTaskRepo>()
                .AddScoped<ImportShopifyOrderService>()
                .AddScoped<ICISUserService, CISUserService>()
                .AddStoreFeature()
                .AddProductFeature()
                .AddOrderFeature()
                .AddInfrastructure();
        }

        public static IdentityBuilder AddCISAuthentication(
            this IServiceCollection services)
        {
            return services
                .AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<CISDbContext>();
        }

        public static IServiceCollection AddSWNDistroLegacyDatabase(
            this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SWNDistroContext>(
                options => options.UseSqlServer(connectionString));

            return services;
        }

        public static async Task ApplyCISDataMigrations(
            this IServiceProvider provider, 
            bool requiresMigrationFromLegacy,
            bool insertTestUser)
        {
            var appDbContext = provider
                .GetRequiredService<CISDbContext>();

            try
            {
                appDbContext.Database.Migrate();

                if(requiresMigrationFromLegacy)
                {
                    if(!appDbContext.MigrationsTasks.Any())
                    {
                        var migrationTaskTypes = Enum.GetValues<MigrationTask.TaskType>();

                        foreach(var type in migrationTaskTypes)
                        {
                            appDbContext.MigrationsTasks.Add(new()
                            {
                                Type = type,
                                Executed = false
                            });
                        }

                        appDbContext.SaveChanges();
                    }
                }

                if(insertTestUser)
                {
                    if (!appDbContext.Roles.Any())
                    {
                        var customerRole = new IdentityRole()
                        {
                            Id = UserTypes.CUSTOMER,
                            Name = UserTypes.CUSTOMER,
                            NormalizedName = UserTypes.CUSTOMER.Normalize()
                        };

                        var adminRole = new IdentityRole()
                        {
                            Id = UserTypes.ADMINISTRATOR,
                            Name = UserTypes.ADMINISTRATOR,
                            NormalizedName = UserTypes.ADMINISTRATOR.Normalize()
                        };

                        appDbContext.Roles.Add(customerRole);
                        appDbContext.Roles.Add(adminRole);
                        appDbContext.SaveChanges();
                    }


                    if (!appDbContext.Users.Any())
                    {
                        var userManager = provider
                            .GetRequiredService<UserManager<ApplicationUser>>();

                        var user = new ApplicationUser
                        {
                            UserName = "admin@cis.no",
                            Email = "admin@cis.no",
                            LockoutEnabled = false,
                            TwoFactorEnabled = false,
                        };
                        var result = await userManager.CreateAsync(user, "Password1.");
                        var emailConfirmationCode = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmEmailRes = await userManager.ConfirmEmailAsync(user, emailConfirmationCode);

                        var role = new IdentityUserRole<string>()
                        {
                            RoleId = UserTypes.ADMINISTRATOR,
                            UserId = user.Id
                        };

                        appDbContext.UserRoles.Add(role);
                        appDbContext.SaveChanges();
                    }
                }
            }
            catch(Exception e)
            {

            }
        }
    }
}
