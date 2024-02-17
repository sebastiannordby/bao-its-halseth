using CIS.Application.Customers.Models;
using CIS.Application.Legacy;
using CIS.Application.Orders.Contracts;
using CIS.Application.Orders.Contracts.Import;
using CIS.Application.Orders.Import;
using CIS.Application.Orders.Import.Contracts;
using CIS.Application.Orders.Repositories;
using CIS.Application.Orders.Services;
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

namespace CIS.Application.Orders
{
    internal static class OrderContextInstaller
    {
        internal static IServiceCollection AddOrderServices(
            this IServiceCollection services)
        {
            services
                .AddScoped<IMigrationMapper<IEnumerable<OrderGroupingStruct>, IEnumerable<SalesOrderImportDefinition>>, LegacyCISSalesOrderMapper>()
                .AddScoped<IProcessImportCommandService<ImportSalesOrderCommand>, ProcessImportSalesOrderCommandService>()
                .AddScoped<IMigrateLegacyService<Ordre>, MigrateLegacyOrderService>()
                .AddScoped<IValidator<ImportSalesOrderCommand>, ImportSalesOrderCommandValidator>()
                .AddScoped<IValidator<SalesOrderImportDefinition>, SalesOrderImportDefinitionValidator>();

            return services
                .AddScoped<ISalesQueries, SalesQueries>()
                .AddScoped<IExecuteImportService<SalesStatisticsImportDefinition>, ImportSalesStatisticsService>()
                .AddScoped<IMigrateLegacyService<Salg>, ImportSalesStatisticsService>();
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
            });

            return modelBuilder;

        }
    }
}
