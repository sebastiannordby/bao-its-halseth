using CIS.Library.Products.Models;
using CIS.Library.Products.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Products.Repositories
{
    internal class ProductViewRepository : IProductViewRepository
    {
        private readonly CISDbContext _dbContext;

        public ProductViewRepository(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<ProductView>> List()
        {
            var productsViewQuery = (
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
            );

            var result = await productsViewQuery.ToListAsync();

            return result.AsReadOnly();
        }
    }
}
