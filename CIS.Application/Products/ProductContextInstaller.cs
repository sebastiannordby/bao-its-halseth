using CIS.Application.Products.Models;
using CIS.Application.Products.Repositories;
using CIS.Application.Products.Services;
using CIS.Library.Products.Import;
using CIS.Library.Products.Repositories;
using CIS.Library.Shared.Services;
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
        internal static IServiceCollection AddProductServices(
            this IServiceCollection services)
        {
            return services
                .AddScoped<IExecuteImportService<ProductImportDefinition>, ImportProductService>()
                .AddScoped<IProductViewRepository, ProductViewRepository>();
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
