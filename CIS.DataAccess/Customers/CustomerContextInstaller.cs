using CIS.DataAccess.Customers.Models;
using CIS.DataAccess.Customers.Repositories;
using CIS.DataAccess.Customers.Services;
using CIS.Domain.Customers.Services;
using CIS.Library.Customers.Models.Import;
using CIS.Library.Customers.Repositories;
using CIS.Library.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Customers
{
    internal static class CustomerContextInstaller
    {
        internal static IServiceCollection AddCustomerServices(
            this IServiceCollection services)
        {
            return services
                .AddScoped<IExecuteImportService<CustomerImportDefinition>, ImportCustomerService>()
                .AddScoped<ICustomerViewRepository, CustomerViewRepository>();
        }

        internal static ModelBuilder SetupCustomerModels(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerDao>(entity =>
            {
                entity.HasKey(x => x.Number);

                entity.HasOne<CustomerGroupDao>()
                  .WithMany()
                  .HasForeignKey(cg => cg.CustomerGroupNumber);
            });

            modelBuilder.Entity<CustomerGroupDao>(entity =>
            {
                entity.HasKey(x => x.Number);
            });

            return modelBuilder;
        }
    }
}
