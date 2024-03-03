using CIS.Library.Shared.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CIS.Application.Features.Orders.Import.Contracts;
using CIS.Application.Features.Orders.Infrastructure.Models;

namespace CIS.Application.Features.Orders.Import
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

                    orders.Add(orderDao);

                    foreach (var lineDefinition in orderDefinition.Lines)
                    {
                        var orderLineDao = new SalesOrderLineDao()
                        {
                            SalesOrderId = orderDao.Id,
                            ProductNumber = lineDefinition.ProductNumber,
                            ProductName = lineDefinition.ProductName,
                            EAN = lineDefinition.EAN,
                            Quantity = (int) lineDefinition.Quantity,
                            QuantityDelivered = (int) lineDefinition.QuantityDelivered,
                            CostPrice = lineDefinition.CostPrice,
                            PurchasePrice = lineDefinition.PurchasePrice,
                            StorePrice = lineDefinition.StorePrice,
                            CurrencyCode = "NOK"
                        };

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
    }
}
