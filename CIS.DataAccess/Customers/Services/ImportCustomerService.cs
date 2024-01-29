using CIS.DataAccess.Customers.Models;
using CIS.DataAccess.Stores.Models;
using CIS.Domain.Customers.Services;
using CIS.Library.Customers.Models.Import;
using CIS.Library.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Customers.Services
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
                    CustomerGroupNumber = customerDefinition.CustomerGroupNumber,
                    IsActive = customerDefinition.IsActive
                };

                await _dbContext.Customers.AddAsync(customerDao);
                await _dbContext.SaveChangesAsync();

                if (customerDefinition.Store is not null)
                {
                    var storeDefinition = customerDefinition.Store;

                    var storeDao = new StoreDao()
                    {
                        Number = customerDefinition.Number,
                        Name = customerDefinition.Name,
                        AddressLine = storeDefinition.AddressLine,
                        AddressPostalCode = storeDefinition.AddressPostalCode,
                        AddressPostalOffice = storeDefinition.AddressPostalOffice,
                        IsActive = true,
                        RegionId = null,
                        OwnerCustomerNumber = customerDao.Number
                    };

                    await _dbContext.Stores.AddAsync(storeDao);
                    await _dbContext.SaveChangesAsync();
                }
            }

            return true;
        }
    }
}
