using CIS.DataAccess.Orders.Models;
using CIS.Library.Orders.Models.Import;
using CIS.Library.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Orders.Services
{
    internal class ImportOrderService : IExecuteImportService<SalesOrderImportDefinition>
    {
        private readonly CISDbContext _dbContext;

        public ImportOrderService(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Import(IEnumerable<SalesOrderImportDefinition> definitions)
        {
            foreach(var orderDefinition in definitions)
            {
                var orderDao = new SalesOrderDao()
                {
                    Number = orderDefinition.Number,
                    AlternateNumber = orderDefinition.AlternateNumber,
                    OrderDate = orderDefinition.OrderDate,
                    Reference = orderDefinition.Reference,
                    DeliveredDate = orderDefinition.DeliveredDate,
                    StoreNumber = orderDefinition.StoreNumber,
                    StoreName = orderDefinition.StoreName,
                    CustomerNumber = orderDefinition.CustomerNumber,
                    CustomerName = orderDefinition.CustomerName,
                    IsDeleted = orderDefinition.IsDeleted
                };

                await _dbContext.AddAsync(orderDao);

                foreach(var lineDefinition in orderDefinition.Lines)
                {
                    var orderLineDao = new SalesOrderLineDao()
                    {
                        SalesOrderId = orderDao.Id,
                        ProductNumber = lineDefinition.ProductNumber,
                        ProductName = lineDefinition.ProductName,
                        EAN = lineDefinition.EAN,
                        Quantity = lineDefinition.Quantity,
                        QuantityDelivered = lineDefinition.QuantityDelivered,
                        CostPrice = lineDefinition.CostPrice,
                        PurchasePrice = lineDefinition.PurchasePrice,
                        StorePrice = lineDefinition.StorePrice,
                        CurrencyCode = "NOK"
                    };

                    await _dbContext.SalesOrderLines.AddAsync(orderLineDao);
                }

                await _dbContext.SaveChangesAsync();
            }

            return true;
        }
    }
}
