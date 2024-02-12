using CIS.Application.Products.Models;
using CIS.Library.Products.Import;
using CIS.Library.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Products.Services
{
    internal class ImportProductService : IExecuteImportService<ProductImportDefinition>
    {
        private readonly CISDbContext _dbContext;

        public ImportProductService(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Import(IEnumerable<ProductImportDefinition> definitions)
        {
            var products = new List<ProductDao>();
            var productPrices = new List<ProductPriceDao>();
            var productGroups = new List<ProductGroupDao>();

            foreach(var definition in definitions)
            {
                var productPrice = new ProductPriceDao() 
                { 
                    Id = Guid.NewGuid(),
                    CostPrice = definition.CostPrice,
                    CurrencyCode = definition.CurrencyCode,
                    PurchasePrice = definition.PurchasePrice,
                    StorePrice = definition.StorePrice,
                };

                productPrices.Add(productPrice);

                var ignoreGroup = definition.ProductGroupNumber.HasValue == false;
                var productGroup = !ignoreGroup ? await _dbContext.ProductGroups
                    .AsNoTracking()
                    .Where(x =>
                        x.Number == definition.Number ||
                        x.Name == definition.Name)
                    .FirstOrDefaultAsync() : null;
                if (productGroup != null && !ignoreGroup && 
                    definition.ProductGroupNumber.HasValue && 
                    !string.IsNullOrWhiteSpace(definition.ProductGroupName)) 
                {
                    productGroup = new ProductGroupDao()
                    {
                        Id = Guid.NewGuid(),
                        Number = definition.ProductGroupNumber.Value,
                        Name = definition.ProductGroupName,
                    };

                    productGroups.Add(productGroup);
                }

                var product = new ProductDao() 
                { 
                    Number = definition.Number,
                    AlternateNumber = definition.AlternateNumber,
                    Name = definition.Name,
                    AlternateName = definition.AlternateName,
                    SuppliersProductNumber = definition.SuppliersProductNumber,
                    EAN = definition.EAN ?? "IKKE DEFINERT",
                    IsActive = definition.IsActive,
                    ProductPriceId = productPrice.Id,
                    ProductGroupId = productGroup?.Id,
                };

                products.Add(product);
            }

            await _dbContext.ProductPrices.AddRangeAsync(productPrices);
            await _dbContext.ProductGroups.AddRangeAsync(productGroups);
            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
