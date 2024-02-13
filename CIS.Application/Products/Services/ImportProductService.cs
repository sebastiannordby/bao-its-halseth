using CIS.Application.Legacy;
using CIS.Application.Products.Models;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Library.Products.Import;
using CIS.Library.Shared.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CIS.Application.Products.Services
{
    internal class ImportProductService : 
        IExecuteImportService<ProductImportDefinition>,
        IMigrateLegacyService<Vareinfo>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _swnDistroContext;

        public ImportProductService(
            CISDbContext dbContext, 
            SWNDistroContext swnDistroContext)
        {
            _dbContext = dbContext;
            _swnDistroContext = swnDistroContext;
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

        public async Task Migrate(Func<string, Task> log)
        {
            await log("Importering av varer påbegynt.");

            await _swnDistroContext.Vareinfos.ProcessEntitiesInBatches(async (products, percentage) =>
            {
                var importDefinitions = new List<ProductImportDefinition>();

                foreach (var legProd in products)
                {
                    var importDef = new ProductImportDefinition()
                    {
                        Number = (int)legProd.Id,
                        AlternateNumber = legProd.VarenrSwn,
                        Name = legProd.Varebeskrivelse2,
                        AlternateName = legProd.VaretekstAlternativ,
                        SuppliersProductNumber = legProd.VarenrLev,
                        EAN = legProd.Ean,
                        IsActive = legProd.Aktiv ?? false,
                        CurrencyCode = "NOK",
                        CostPrice = legProd.OurPrice,
                        PurchasePrice = legProd.Innpris,
                        StorePrice = legProd.Utpris
                    };

                    importDefinitions.Add(importDef);
                }

                var success = await Import(importDefinitions);
                var productNames = importDefinitions
                    .Select(x => x.Name)
                    .ToArray();

                var productNamesMsg = string.Join("\n", productNames);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({percentage}%)({successMsg}) Varer:\n{productNamesMsg}\n";

                await log(message);
            });

            await log("Importering av varer vellykket.");
        }
    }
}
