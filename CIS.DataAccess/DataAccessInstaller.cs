using CIS.DataAccess.Customers.Repositories;
using CIS.DataAccess.Repositories;
using CIS.DataAccess.Services;
using CIS.Domain.Customers.Services;
using CIS.Library.Customers.Repositories;
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
            services.AddDbContext<CISDbContext>(factory);

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerViewRepository, CustomerViewRepository>();

            return services;
        }
    }
}
