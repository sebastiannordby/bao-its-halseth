using CIS.Application.Customers;
using CIS.Application.Customers.Repositories;
using CIS.Application.Legacy;
using CIS.Application.Orders;
using CIS.Application.Products;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using CIS.Application.Shopify;
using CIS.Application.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CIS.Application
{
    public static class DataAccessInstaller
    {
        public static IServiceCollection AddDataAccess(
            this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CISDbContext>(options =>
                options.UseSqlServer(connectionString), ServiceLifetime.Transient);

            return services
                .AddScoped<IMigrationTaskRepo, MigrationTaskRepo>()
                .AddScoped<ImportShopifyOrderService>()
                .AddCustomerServices()
                .AddStoreServices()
                .AddProductServices()
                .AddOrderServices();
        }

        public static IServiceCollection AddLegacyDatabase(
            this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SWNDistroContext>(
                options => options.UseSqlServer(connectionString));

            return services;
        }

        public static async Task MigrateDataAccess(
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
