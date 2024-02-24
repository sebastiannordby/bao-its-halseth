using CIS.Application.Legacy;
using CIS.Application.Products.Import;
using CIS.Application.Products.Import.Contracts;
using CIS.Application.Products.Migration;
using CIS.Application.Products.Migration.Contracts;
using CIS.Application.Products.Models;
using CIS.Application.Shared.Infrastructure;
using CIS.Application.Shared.Services;
using CIS.Library.Products.Import;
using CIS.Library.Shared.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Products
{
    internal static class ProductContextInstaller
    {
        internal static IServiceCollection AddProductFeature(
            this IServiceCollection services)
        {

            services
                .AddScoped<IProcessImportCommandService<ImportProductCommand>, ProcessImportProductCommandService>()
                .AddScoped<IValidator<ImportProductCommand>, ImportProductCommandValidator>()
                .AddScoped<IValidator<ImportProductDefinition>, ImportProductDefinitionValidator>();

            services
                .AddScoped<IMigrationMapper<LegacySystemProductSource, ImportProductDefinition>, LegacySystemProductMapper>()
                .AddScoped<IMigrateLegacyService<Vareinfo>, MigrateLegacyProductService>();

            return services;
        }

        internal static ModelBuilder SetupProductModels(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductDao>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .HasMaxLength(300);
                entity.Property(x => x.SuppliersProductNumber)
                    .HasMaxLength(20);
                entity.Property(x => x.EAN)
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<ProductGroupDao>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ProductPriceDao>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.PurchasePrice).HasColumnType("decimal(18, 5)");
                entity.Property(x => x.CostPrice).HasColumnType("decimal(18, 5)");
                entity.Property(x => x.StorePrice).HasColumnType("decimal(18, 5)");
            });

            return modelBuilder;
        }
    }
}
