using CIS.DataAccess.Customers;
using CIS.DataAccess.Customers.Repositories;
using CIS.DataAccess.Products;
using CIS.DataAccess.Stores;
using CIS.DataAccess.Stores.Repositories;
using CIS.Domain.Customers.Services;
using CIS.Library.Customers.Repositories;
using CIS.Library.Stores.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .AddProductServices();
        }

        public static void MigrateDataAccess(this IServiceProvider provider)
        {
            var appDbContext = provider
                .GetRequiredService<CISDbContext>();

            try
            {
                appDbContext.Database.Migrate();
            }
            catch (Exception)
            {

            }
        }
    }
}
