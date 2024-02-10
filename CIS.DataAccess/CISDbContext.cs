using CIS.Application.Customers;
using CIS.Application.Customers.Models;
using CIS.Application.Orders;
using CIS.Application.Orders.Models;
using CIS.Application.Products;
using CIS.Application.Products.Models;
using CIS.Application.Shared.Models;
using CIS.Application.Stores;
using CIS.Application.Stores.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application
{
    internal sealed class CISDbContext : DbContext
    {
        public required DbSet<CustomerDao> Customers { get; set; }
        public required DbSet<CustomerGroupDao> CustomerGroups { get; set; }

        public required DbSet<ProductDao> Products { get; set; }
        public required DbSet<ProductPriceDao> ProductPrices { get; set; }
        public required DbSet<ProductGroupDao> ProductGroups { get; set; }

        public required DbSet<StoreDao> Stores { get; set; }
        public required DbSet<RegionDao> Regions { get; set; }

        public required DbSet<SalesOrderDao> SalesOrders { get; set; }
        public required DbSet<SalesOrderLineDao> SalesOrderLines { get; set; }

        public required DbSet<MigrationTask> MigrationsTasks { get; set; }

        public CISDbContext() : base()
        {
        
        } 

        public CISDbContext(DbContextOptions<CISDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SetupCustomerModels();
            modelBuilder.SetupOrderModels();
            modelBuilder.SetupProductModels();
            modelBuilder.SetupStoreModels();
        }
    }
}
