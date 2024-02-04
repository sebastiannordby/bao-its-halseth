using CIS.DataAccess.Stores.Models;
using CIS.DataAccess.Stores.Repositories;
using CIS.Library.Stores.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Stores
{
    internal static class StoreContextInstaller
    {
        internal static IServiceCollection AddStoreServices(
            this IServiceCollection services)
        {
            return services
                .AddScoped<IStoreViewRepository, StoreViewRepository>();
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

            return modelBuilder;
        }
    }
}
