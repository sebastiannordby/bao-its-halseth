using CIS.DataAccess.Products.Repositories;
using CIS.DataAccess.Products.Services;
using CIS.Domain.Products.Models.Import;
using CIS.Library.Products.Repositories;
using CIS.Library.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Products
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
    }
}
