using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Products.Import.Contracts;
using CIS.Application.Products.Models;
using CIS.Library.Shared.Services;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Products.Import
{
    internal class ProcessImportProductCommandService :
        IProcessImportCommandService<ImportProductCommand>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _swnDistroContext;
        private readonly IValidator<ImportProductCommand> _commandValidator;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hub;

        public ProcessImportProductCommandService(
            CISDbContext dbContext,
            SWNDistroContext swnDistroContext,
            IValidator<ImportProductCommand> commandValidator,
            IHubContext<ImportLegacyDataHub, IListenImportClient> hub)
        {
            _dbContext = dbContext;
            _swnDistroContext = swnDistroContext;
            _commandValidator = commandValidator;
            _hub = hub;
        }

        public async Task<bool> Import(ImportProductCommand command, CancellationToken cancellationToken)
        {
            await _commandValidator.ValidateAndThrowAsync(command);

            var definitions = command.Definitions;
            var products = new List<ProductDao>();
            var productPrices = new List<ProductPriceDao>();
            var productGroups = new List<ProductGroupDao>();

            foreach (var definition in definitions)
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
