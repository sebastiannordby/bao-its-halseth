using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using CIS.Application;
using CIS.Library.Shared.Services;
using CIS.Library.Products.Import;

namespace CIS.UnitTesting.Application.Products.Services
{
    public class ProductImportTests : DataAccessTest
    {
        [Test]
        public void ServiceAvailabilityDI()
        {
            var service = Services.GetRequiredService<IExecuteImportService<ProductImportDefinition>>();

            Assert.IsNotNull(service);
        }

        [Test]
        public async Task TestImportAsync()
        {
            var service = Services
                .GetRequiredService<IExecuteImportService<ProductImportDefinition>>();
            var dbContext = Services.GetRequiredService<CISDbContext>();
            var definitions = new List<ProductImportDefinition>()
            {
                new ProductImportDefinition()
                {
                    Number = 1,
                    Name = "Ladekabel Lightning",
                    AlternateName = "Ladekabel Apple",
                    ProductGroupNumber = 1,
                    ProductGroupName = "Wiwala",
                    CostPrice = 11,
                    PurchasePrice = 22,
                    StorePrice = 33,
                    CurrencyCode = "NOK"
                }
            };

            var result = await service.Import(definitions);

            var productCount = dbContext.Products
                .Count();

            Assert.IsTrue(result);
            Assert.AreEqual(productCount, definitions.Count);
        }
    }
}
