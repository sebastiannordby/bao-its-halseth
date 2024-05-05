using CIS.Application.Features.Products.Import.Contracts;
using CIS.Application.Features.Products.Models;
using CIS.Library.Shared.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CIS.Application.Features.Products.Import
{
    internal class ProcessImportProductCommandService :
        IProcessImportCommandService<ImportProductCommand>
    {
        private readonly CISDbContext _dbContext;
        private readonly IValidator<ImportProductCommand> _commandValidator;

        public ProcessImportProductCommandService(
            CISDbContext dbContext,
            IValidator<ImportProductCommand> commandValidator)
        {
            _dbContext = dbContext;
            _commandValidator = commandValidator;
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
