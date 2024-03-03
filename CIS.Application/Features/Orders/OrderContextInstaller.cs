using CIS.Application.Legacy;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CIS.Application.Shared.Infrastructure;
using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Import;
using CIS.Application.Features.Orders.Migration;
using CIS.Application.Features.Orders.Import.Contracts;
using CIS.Application.Features.Orders.Infrastructure.Models;
using CIS.Application.Features.Orders.Migration.Contracts;

namespace CIS.Application.Features.Orders
{
    internal static class OrderContextInstaller
    {
        internal static IServiceCollection AddOrderFeature(
            this IServiceCollection services)
        {

            // Migration-related
            services
                .AddScoped<IMigrationMapper<LegacySystemSalesOrderSource, ImportSalesOrderDefinition>, LegacySystemSalesOrderMapper>()
                .AddScoped<IMigrateLegacyService<Salg>, MigrateLegacySalgService>();

            services
                .AddScoped<IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition>, LegacySystemSalesStatisticsMapper>()
                .AddScoped<IMigrateLegacyService<Ordre>, MigrateLegacyOrderService>();

            // Import-related
            services
                .AddScoped<IValidator<ImportSalesOrderCommand>, ImportSalesOrderCommandValidator>()
                .AddScoped<IValidator<ImportSalesOrderDefinition>, ImportSalesOrderDefinitionValidator>()
                .AddScoped<IProcessImportCommandService<ImportSalesOrderCommand>, ProcessImportSalesOrderCommandService>();

            services
                .AddScoped<IValidator<ImportSalesStatisticsCommand>, ImportSalesStatisticsCommandValidator>()
                .AddScoped<IValidator<ImportSalesStatisticsDefinition>, ImportSalesStatisticsDefinitionValidator>()
                .AddScoped<IProcessImportCommandService<ImportSalesStatisticsCommand>, ProcessImportSalesStatisticsCommandService>();

            return services;
        }

        internal static ModelBuilder SetupOrderModels(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SalesOrderDao>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity
                    .Property(x => x.AlternateNumber)
                    .HasMaxLength(30);
                entity
                    .Property(x => x.Reference)
                    .HasMaxLength(100);
                entity
                    .Property(x => x.StoreNumber)
                    .HasMaxLength(150);
                entity
                    .Property(x => x.CustomerName)
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<SalesOrderLineDao>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne<SalesOrderDao>()
                  .WithMany()
                  .HasForeignKey(cg => cg.SalesOrderId);
                entity
                    .Property(x => x.ProductName)
                    .HasMaxLength(300);
                entity
                    .Property(x => x.EAN)
                    .HasMaxLength(30);
                entity
                    .Property(x => x.CurrencyCode)
                    .HasMaxLength(3);
                entity
                    .Property(x => x.PurchasePrice)
                    .HasColumnType("decimal(18, 5)");
                entity
                    .Property(x => x.CostPrice)
                    .HasColumnType("decimal(18, 5)");
                entity
                    .Property(x => x.StorePrice)
                    .HasColumnType("decimal(18, 5)");
                entity
                    .Property(x => x.Quantity)
                    .HasColumnType("decimal(18, 5)");
                entity
                    .Property(x => x.QuantityDelivered)
                    .HasColumnType("decimal(18, 5)");
            });

            return modelBuilder;

        }
    }
}
