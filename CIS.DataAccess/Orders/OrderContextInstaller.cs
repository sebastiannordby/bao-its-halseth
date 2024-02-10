﻿using CIS.Application.Customers.Models;
using CIS.Application.Orders.Models;
using CIS.Application.Orders.Models.Import;
using CIS.Application.Orders.Repositories;
using CIS.Application.Orders.Services;
using CIS.Library.Orders.Models.Import;
using CIS.Library.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders
{
    internal static class OrderContextInstaller
    {
        internal static IServiceCollection AddOrderServices(
            this IServiceCollection services)
        {
            return services
                .AddScoped<ISalesOrderViewRepository, SalesOrderViewRepository>()
                .AddScoped<IExecuteImportService<SalesOrderImportDefinition>, ImportOrderService>()
                .AddScoped<IExecuteImportService<SalesStatisticsImportDefinition>, ImportSalesStatisticsService>();
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
