using CIS.Application.Products;
using CIS.Library.Products.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Infrastructure
{
    internal class ProductQueries : IProductQueries
    {
        private readonly CISDbContext _dbContext;

        public ProductQueries(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<ProductView>> List(CancellationToken cancellationToken = default)
        {
            var productsViewQuery = await ( 
                from product in _dbContext.Products
                    .AsNoTracking()
                from productPrice in _dbContext.ProductPrices
                    .Where(x =>
                        product.ProductPriceId.HasValue &&
                        product.ProductPriceId == x.Id)
                    .DefaultIfEmpty()
                from productGroup in _dbContext.ProductGroups
                    .Where(x =>
                        product.ProductGroupId.HasValue &&
                        product.ProductGroupId == x.Id)
                    .DefaultIfEmpty()

                select new ProductView(
                    product.Number,
                    product.Name,
                    product.AlternateName,
                    productPrice != null ? productPrice.CostPrice : null,
                    productPrice != null ? productPrice.PurchasePrice : null,
                    productPrice != null ? productPrice.StorePrice : null,
                    productGroup != null ? productGroup.Number : null,
                    productGroup != null ? productGroup.Name : null
                )
            ).ToListAsync();

            return productsViewQuery;
        }
    }
}
