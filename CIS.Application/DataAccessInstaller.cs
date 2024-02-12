using CIS.Application.Customers;
using CIS.Application.Customers.Repositories;
using CIS.Application.Legacy;
using CIS.Application.Orders;
using CIS.Application.Products;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using CIS.Application.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CIS.Application
{
    public static class DataAccessInstaller
    {
        public static IServiceCollection AddDataAccess(
            this IServiceCollection services, Action<DbContextOptionsBuilder>? factory)
        {
            return services
                .AddDbContext<CISDbContext>(factory)
                .AddScoped<IMigrationTaskRepo, MigrationTaskRepo>()
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

        public static void MigrateDataAccess(
            this IServiceProvider provider, bool requiresMigrationFromLegacy)
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
            }
            catch(Exception e)
            {

            }
        }
    }
}
