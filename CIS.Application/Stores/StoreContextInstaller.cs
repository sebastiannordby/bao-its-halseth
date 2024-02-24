 using CIS.Application.Legacy;
using CIS.Application.Shared.Services;
using CIS.Application.Stores.Import;
using CIS.Application.Stores.Import.Contracts;
using CIS.Application.Stores.Migration;
using CIS.Application.Stores.Migration.Contracts;
using CIS.Application.Stores.Models;
using CIS.Application.Stores.Models.Import;
using CIS.Application.Stores.Services;
using CIS.Application.Stores.Services.Implementation;
using CIS.Library.Shared.Services;
using CIS.Library.Stores.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Stores
{
    internal static class StoreContextInstaller
    {
        internal static IServiceCollection AddStoreFeature(
            this IServiceCollection services)
        {
            // Migration-related
            services
                .AddScoped<IMigrateLegacyService<Butikkliste>, MigrateLegacyCustomerService>()
                .AddScoped<IMigrationMapper<LegacySystemCustomerSource, ImportCustomerDefinition>, LegacySystemCustomerMapper>();

            services
                .AddScoped<IProcessImportCommandService<ImportCustomerCommand>, ProcessImportCustomerCommandService>()
                .AddScoped<IValidator<ImportCustomerCommand>, ImportCustomerCommandValidator>()

                .AddScoped<IValidator<ImportCustomerDefinition>, ImportCustomerDefinitionValidator>();

            return services
                .AddScoped<IStoreService, StoreService>()
                .AddScoped<IStockCountService, StockCountService>();
        }

        internal static ModelBuilder SetupStoreModels(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegionDao>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<StoreDao>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<StockCountDao>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

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
