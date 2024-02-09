using CIS.DataAccess.Customers;
using CIS.DataAccess.Customers.Models;
using CIS.DataAccess.Orders;
using CIS.DataAccess.Orders.Models;
using CIS.DataAccess.Products;
using CIS.DataAccess.Products.Models;
using CIS.DataAccess.Stores;
using CIS.DataAccess.Stores.Models;
using CIS.Domain.Orders.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess
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
