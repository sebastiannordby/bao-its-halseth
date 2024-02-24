using CIS.Application.Orders.Import.Contracts;
using CIS.Application.Orders.Infrastructure.Models;
using CIS.Library.Shared.Services;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Import
{
    internal class ProcessImportSalesStatisticsCommandService :
        IProcessImportCommandService<ImportSalesStatisticsCommand>
    {
        private readonly CISDbContext _dbContext;
        private readonly IValidator<ImportSalesStatisticsCommand> _commandValidator;

        public ProcessImportSalesStatisticsCommandService(
            CISDbContext dbContext,
            IValidator<ImportSalesStatisticsCommand> commandValidator)
        {
            _dbContext = dbContext;
            _commandValidator = commandValidator;
        }

        public async Task<bool> Import(ImportSalesStatisticsCommand command, CancellationToken cancellationToken)
        {
            await _commandValidator.ValidateAndThrowAsync(command, cancellationToken);

            var definitions = command.Definitions;
            var statistics = new List<SalesStatisticsDao>();

            foreach (var definition in definitions)
            {
                var dao = new SalesStatisticsDao()
                {
                    Id = Guid.NewGuid(),
                    Number = definition.Number,
                    Date = definition.Date,
                    CostPrice = definition.CostPrice,
                    ProductNumber = definition.ProductNumber,
                    CustomerNumber = definition.CustomerNumber,
                    PurchasePrice = definition.PurchasePrice,
                    Quantity = definition.Quantity,
                    StoreNumber = definition.StoreNumber,
                    StorePrice = definition.StorePrice,
                };

                statistics.Add(dao);
            }

            await _dbContext.SalesStatistics.AddRangeAsync(statistics);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
