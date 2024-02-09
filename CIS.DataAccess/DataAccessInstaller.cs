using CIS.DataAccess.Customers;
using CIS.DataAccess.Customers.Repositories;
using CIS.DataAccess.Legacy;
using CIS.DataAccess.Orders;
using CIS.DataAccess.Products;
using CIS.DataAccess.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CIS.DataAccess
{
    public static class DataAccessInstaller
    {
        public static IServiceCollection AddDataAccess(
            this IServiceCollection services, Action<DbContextOptionsBuilder>? factory)
        {
            return services
                .AddDbContext<CISDbContext>(factory)
                .AddCustomerServices()
                .AddStoreServices()
                .AddProductServices()
                .AddOrderServices();
        }

        public static IServiceCollection AddLegacyDatabase(
            this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SWNDistro>(
                options => options.UseSqlServer(connectionString));

            return services;
        }

        public static void MigrateDataAccess(this IServiceProvider provider)
        {
            var appDbContext = provider
                .GetRequiredService<CISDbContext>();

            try
            {
                appDbContext.Database.Migrate();
            }
            catch(Exception e)
            {

            }
        }
    }
}
