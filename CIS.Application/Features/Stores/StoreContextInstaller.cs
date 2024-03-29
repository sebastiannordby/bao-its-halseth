﻿using CIS.Application.Features.Stores.Import;
using CIS.Application.Features.Stores.Import.Contracts;
using CIS.Application.Features.Stores.Migration;
using CIS.Application.Features.Stores.Migration.Contracts;
using CIS.Application.Features.Stores.Models;
using CIS.Application.Features.Stores.Models.Import;
using CIS.Application.Features.Stores.Services;
using CIS.Application.Features.Stores.Services.Implementation;
using CIS.Application.Legacy;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Stores
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
