using CIS.Application.Customers.Models;
using CIS.Application.Stores.Models;
using CIS.Library.Customers.Models.Import;
using CIS.Library.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Customers.Services
{
    internal class ImportCustomerService : IExecuteImportService<CustomerImportDefinition>
    {
        private readonly CISDbContext _dbContext;

        public ImportCustomerService(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Import(IEnumerable<CustomerImportDefinition> importDefinitions)
        {
            var lookedUpRegions = new List<RegionImportStruct>();

            foreach (var customerDefinition in importDefinitions)
            {
                var doesExist = await _dbContext.Customers
                    .AnyAsync(x => x.Number == customerDefinition.Number);
                if (doesExist)
                    continue;

                var customerDao = new CustomerDao()
                {
                    Number = customerDefinition.Number,
                    Name = customerDefinition.Name,
                    ContactPersonEmailAddress = customerDefinition.ContactPersonEmailAddress,
                    ContactPersonName = customerDefinition.ContactPersonName,
                    ContactPersonPhoneNumber = customerDefinition.ContactPersonPhoneNumber,
                    CustomerGroupId = null,
                    IsActive = customerDefinition.IsActive
                };

                await _dbContext.Customers.AddAsync(customerDao);

                if (customerDefinition.Store is not null)
                {
                    var storeDefinition = customerDefinition.Store;
                    var regionId = null as Guid?;

                    if(storeDefinition.RegionNumber.HasValue)
                    {
                        var regionExists = _dbContext.Regions
                            .Any(x => x.Number == storeDefinition.RegionNumber);
                        if(!regionExists)
                        {
                            var regionDao = new RegionDao()
                            {
                                Number = storeDefinition.RegionNumber.Value,
                                Name = storeDefinition.Name ?? "IKKE DEFINERT",
                            };

                            await _dbContext.Regions.AddAsync(regionDao);

                            regionId = regionDao.Id;
                        }
                    }

                    var storeDao = new StoreDao()
                    {
                        Number = customerDefinition.Number,
                        Name = customerDefinition.Name,
                        AddressLine = storeDefinition.AddressLine,
                        AddressPostalCode = storeDefinition.AddressPostalCode,
                        AddressPostalOffice = storeDefinition.AddressPostalOffice,
                        IsActive = true,
                        RegionId = regionId,
                        OwnerCustomerId = customerDao.Id
                    };

                    await _dbContext.Stores.AddAsync(storeDao);
                }

                await _dbContext.SaveChangesAsync();
            }

            return true;
        }

        private struct RegionImportStruct
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }
    }
}
