using CIS.DataAccess.Customers;
using CIS.DataAccess.Customers.Models;
using CIS.DataAccess.Products.Models;
using CIS.DataAccess.Stores.Models;
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

        public CISDbContext() : base()
        {
        
        } 

        public CISDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SetupCustomerModels();

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
        }
    }
}
