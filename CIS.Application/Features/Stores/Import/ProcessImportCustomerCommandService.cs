using CIS.Application.Features.Stores.Import.Contracts;
using CIS.Application.Features.Stores.Models;
using CIS.Library.Shared.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CIS.Application.Features.Stores.Import
{
    internal sealed class ProcessImportCustomerCommandService :
        IProcessImportCommandService<ImportCustomerCommand>
    {
        private readonly CISDbContext _dbContext;
        private readonly IValidator<ImportCustomerCommand> _commandValidator;

        public ProcessImportCustomerCommandService(
            CISDbContext dbContext,
            IValidator<ImportCustomerCommand> commandValidator)
        {
            _dbContext = dbContext;
            _commandValidator = commandValidator;
        }

        public async Task<bool> Import(ImportCustomerCommand command, CancellationToken cancellationToken)
        {
            await _commandValidator.ValidateAndThrowAsync(command, cancellationToken);

            var importDefinitions = command.Definitions;
            var lookedUpRegions = new List<RegionImportStruct>();
            var customers = new List<CustomerDao>();
            var stores = new List<StoreDao>();
            var regions = new List<RegionDao>();

            foreach (var customerDefinition in importDefinitions)
            {
                var doesExist = await _dbContext.Customers
                    .AnyAsync(x => x.Number == customerDefinition.Number);
                if (doesExist)
                    continue;

                var customerDao = new CustomerDao()
                {
                    Id = Guid.NewGuid(),
                    Number = customerDefinition.Number,
                    Name = customerDefinition.Name,
                    ContactPersonEmailAddress = customerDefinition.ContactPersonEmailAddress,
                    ContactPersonName = customerDefinition.ContactPersonName,
                    ContactPersonPhoneNumber = customerDefinition.ContactPersonPhoneNumber,
                    CustomerGroupId = null,
                    IsActive = customerDefinition.IsActive
                };

                customers.Add(customerDao);

                if (customerDefinition.Store is not null)
                {
                    var storeDefinition = customerDefinition.Store;
                    var regionId = null as Guid?;

                    if (storeDefinition.RegionNumber.HasValue)
                    {
                        var region = regions
                            .FirstOrDefault(x => x.Number == storeDefinition.RegionNumber);
                        if (region is null)
                        {
                            region = new RegionDao()
                            {
                                Id = Guid.NewGuid(),
                                Number = storeDefinition.RegionNumber.Value,
                                Name = storeDefinition.Name ?? "IKKE DEFINERT",
                            };

                            regions.Add(region);
                        }

                        regionId = region.Id;
                    }

                    var storeDao = new StoreDao()
                    {
                        Id = Guid.NewGuid(),
                        Number = storeDefinition.Number,
                        Name = customerDefinition.Name,
                        AddressLine = storeDefinition.AddressLine,
                        AddressPostalCode = storeDefinition.AddressPostalCode,
                        AddressPostalOffice = storeDefinition.AddressPostalOffice,
                        IsActive = true,
                        RegionId = regionId,
                        OwnerCustomerId = customerDao.Id
                    };

                    stores.Add(storeDao);
                }
            }

            await _dbContext.Customers.AddRangeAsync(customers);
            await _dbContext.Stores.AddRangeAsync(stores);
            await _dbContext.Regions.AddRangeAsync(regions);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        private struct RegionImportStruct
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }
    }
}
