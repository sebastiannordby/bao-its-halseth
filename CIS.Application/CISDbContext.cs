﻿using CIS.Application.Features.Orders;
using CIS.Application.Features.Orders.Infrastructure.Models;
using CIS.Application.Features.Products;
using CIS.Application.Features.Products.Models;
using CIS.Application.Features.Stores;
using CIS.Application.Features.Stores.Models;
using CIS.Application.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application
{
    public sealed class CISDbContext : IdentityDbContext<ApplicationUser>
    {
        public required DbSet<CustomerDao> Customers { get; set; }
        public required DbSet<CustomerGroupDao> CustomerGroups { get; set; }

        public required DbSet<ProductDao> Products { get; set; }
        public required DbSet<ProductPriceDao> ProductPrices { get; set; }
        public required DbSet<ProductGroupDao> ProductGroups { get; set; }

        public required DbSet<StoreDao> Stores { get; set; }
        public required DbSet<RegionDao> Regions { get; set; }
        public required DbSet<StockCountDao> StockCounts { get; set; }

        public required DbSet<SalesOrderDao> SalesOrders { get; set; }
        public required DbSet<SalesOrderLineDao> SalesOrderLines { get; set; }
        public required DbSet<SalesStatisticsDao> SalesStatistics { get; set; }
        
        public required DbSet<MigrationTask> MigrationsTasks { get; set; }
        public required DbSet<LogEntry> LogEntries { get; set; }

        public CISDbContext() : base()
        {
        
        } 

        public CISDbContext(DbContextOptions<CISDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.SetupOrderModels();
            modelBuilder.SetupProductModels();
            modelBuilder.SetupStoreModels();
        }
    }
}
