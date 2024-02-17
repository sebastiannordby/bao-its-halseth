using CIS.Application.Orders.Contracts;
using CIS.Application.Orders.Import.Contracts;
using CIS.Library.Shared.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Import
{
    internal class ProcessImportSalesOrderCommandService :
        IProcessImportCommandService<ImportSalesOrderCommand>
    {
        private Dictionary<int, string> _storeNames = new();
        private Dictionary<int, string> _productNames = new();

        private readonly CISDbContext _dbContext;
        private readonly IValidator<ImportSalesOrderCommand> _commandValidator;

        public ProcessImportSalesOrderCommandService(
            CISDbContext dbContext,
            IValidator<ImportSalesOrderCommand> commandValidator)
        {
            _dbContext = dbContext;
            _commandValidator = commandValidator;
        }

        public async Task<bool> Import(ImportSalesOrderCommand command, CancellationToken cancellationToken)
        {
            await _commandValidator.ValidateAndThrowAsync(command, cancellationToken);

            var definitions = command.Definitions;

            try
            {
                var orders = new List<SalesOrderDao>();
                var orderLines = new List<SalesOrderLineDao>();

                foreach (var orderDefinition in definitions)
                {
                    var orderDao = new SalesOrderDao()
                    {
                        Id = Guid.NewGuid(),
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

                    if (string.IsNullOrWhiteSpace(orderDao.StoreName))
                    {
                        orderDao.StoreName = await GetStoreNameCashed(
                            orderDao.StoreNumber) ?? "";
                    }

                    if (string.IsNullOrWhiteSpace(orderDao.CustomerName))
                    {
                        orderDao.CustomerName = orderDao.StoreName;
                    }

                    orders.Add(orderDao);

                    foreach (var lineDefinition in orderDefinition.Lines)
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

                        if (string.IsNullOrWhiteSpace(orderLineDao.ProductName))
                        {
                            orderLineDao.ProductName = await GetProductNameCached(
                                orderLineDao.ProductNumber, orderLineDao.EAN);
                        }

                        orderLines.Add(orderLineDao);
                    }
                }

                await _dbContext.SalesOrders.AddRangeAsync(orders);
                await _dbContext.SalesOrderLines.AddRangeAsync(orderLines);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task<string> GetProductNameCached(
    int productNumber, string ean)
        {
            if (_productNames.ContainsKey(productNumber))
                return _productNames[productNumber];

            var productName = await _dbContext.Products
                .Where(x =>
                    x.Number == productNumber ||
                    x.EAN == ean)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? string.Empty;
            _productNames.Add(productNumber, productName);

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
