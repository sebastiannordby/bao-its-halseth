using CIS.Application.Customers.Models;
using CIS.Application.Customers.Repositories;
using CIS.Application.Customers.Services;
using CIS.Application.Legacy;
using CIS.Application.Shared.Services;
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

namespace CIS.Application.Customers
{
    internal static class CustomerContextInstaller
    {
        internal static IServiceCollection AddCustomerServices(
            this IServiceCollection services)
        {
            return services
                .AddScoped<IExecuteImportService<CustomerImportDefinition>, ImportCustomerService>()
                .AddScoped<IMigrateLegacyService<Butikkliste>, ImportCustomerService>()
                .AddScoped<ICustomerViewRepository, CustomerViewRepository>();
        }

        internal static ModelBuilder SetupCustomerModels(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerDao>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .HasMaxLength(150);
                entity.Property(x => x.ContactPersonName)
                    .HasMaxLength(150);
                entity.Property(x => x.ContactPersonEmailAddress)
                    .HasMaxLength(100);
                entity.Property(x => x.ContactPersonPhoneNumber)
                    .HasMaxLength(20);

                entity.HasOne<CustomerGroupDao>()
                  .WithMany()
                  .HasForeignKey(cg => cg.CustomerGroupId);
            });

            modelBuilder.Entity<CustomerGroupDao>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .HasMaxLength(100);
            });

            return modelBuilder;
        }
    }
}
