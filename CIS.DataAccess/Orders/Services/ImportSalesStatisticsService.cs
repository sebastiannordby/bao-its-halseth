using CIS.Application.Orders.Models.Import;
using CIS.Library.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CIS.Application.Orders.Services
{
    internal class ImportSalesStatisticsService : IExecuteImportService<SalesStatisticsImportDefinition>
    {
        private readonly CISDbContext _dbContext;

        public ImportSalesStatisticsService(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Import(IEnumerable<SalesStatisticsImportDefinition> definitions)
        {
            foreach(var definition in definitions) 
            {
                await _dbContext.SalesStatistics.AddAsync(new()
                {
                    Number = definition.Number,
                    CostPrice = definition.CostPrice,
                    ProductNumber = definition.ProductNumber,
                    CustomerNumber = definition.CustomerNumber,
                    PurchasePrice = definition.PurchasePrice,
                    Quantity = definition.Quantity,
                    StoreNumber = definition.StoreNumber,
                    StorePrice = definition.StorePrice,
                });

                await _dbContext.SaveChangesAsync();
            }

            return true;
        }
    }
}
