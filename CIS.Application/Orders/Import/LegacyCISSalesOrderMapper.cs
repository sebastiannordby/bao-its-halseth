using CIS.Application.Legacy;
using CIS.Application.Orders.Contracts;
using CIS.Application.Orders.Services;
using CIS.Application.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Import
{
    internal class LegacyCISSalesOrderMapper : IMigrationMapper<IEnumerable<OrderGroupingStruct>, IEnumerable<SalesOrderImportDefinition>>
    {
        private Dictionary<int, string> _storeNames = new();
        private Dictionary<string, string> _productNames = new();

        private readonly SWNDistroContext _swnDbContext;
        private readonly CISDbContext _dbContext;

        public LegacyCISSalesOrderMapper(
            SWNDistroContext swnDbContext, 
            CISDbContext dbContext)
        {
            _swnDbContext = swnDbContext;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<SalesOrderImportDefinition>> Map(IEnumerable<OrderGroupingStruct> orderGroupings)
        {
            var importDefinitions = new List<SalesOrderImportDefinition>();

            foreach (var ordreStruct in orderGroupings)
            {
                var grouping = await _swnDbContext.Ordres
                    .AsNoTracking()
                    .Where(x => x.Butikknr == ordreStruct.Butikknr)
                    .Where(x => x.Dato == ordreStruct.Dato)
                    .ToListAsync();

                var legOrder = grouping.First();
                if (!legOrder.Butikknr.HasValue)
                    throw new Exception();

                var storeName = await GetStoreNameCashed(
                    legOrder.Butikknr.Value);
                if (string.IsNullOrWhiteSpace(storeName))
                    storeName = "IKKE FUNNET";

                var importOrder = new SalesOrderImportDefinition()
                {
                    Number = (int)legOrder.Id,
                    AlternateNumber = legOrder.NettOrdreRef,
                    StoreNumber = legOrder.Butikknr ?? 0,
                    StoreName = storeName,
                    CustomerNumber = legOrder.Butikknr ?? 0,
                    CustomerName = storeName,
                    DeliveredDate = DateOnly.FromDateTime(DateTime.Now),
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    Reference = legOrder.Ordreref as string,
                    IsDeleted = (legOrder.Ordretype ?? "").Equals("Slettet", StringComparison.OrdinalIgnoreCase)
                };

                foreach (var ordreLinje in grouping)
                {
                    var productName = await GetProductNameCached(ordreLinje.Ean);
                    if (string.IsNullOrWhiteSpace(productName))
                        productName = "IKKE FUNNET";

                    var importLine = new SalesOrderImportDefinition.Line()
                    {
                        CostPrice = ordreLinje.OurPrice,
                        EAN = ordreLinje.Ean,
                        ProductName = productName,
                        Quantity = ordreLinje.Antall ?? 0,
                        PurchasePrice = ordreLinje.Innpris,
                        QuantityDelivered = ordreLinje.AntallLevert ?? 0,
                        CurrencyCode = "NOK"
                    };

                    importOrder.Lines.Add(importLine);
                }

                importDefinitions.Add(importOrder);
            }

            return importDefinitions;
        }

        private async Task<string> GetProductNameCached(string ean)
        {
            if (_productNames.ContainsKey(ean))
                return _productNames[ean];

            var productName = await _dbContext.Products
                .Where(x =>x.EAN == ean)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? string.Empty;
            _productNames.Add(ean, productName);

            return productName;
        }

        private async Task<string?> GetStoreNameCashed(int storeNumber)
        {
            if (storeNumber == 0)
                return null;

            if (_storeNames.ContainsKey(storeNumber))
                return _storeNames[storeNumber];

            var storeName = await _dbContext.Stores
                .Where(x =>
                    x.Number == storeNumber)
                .Select(x => x.Name)
            .FirstOrDefaultAsync() ?? string.Empty;

            _storeNames.Add(storeNumber, storeName);

            return storeName;
        }
    }
}
