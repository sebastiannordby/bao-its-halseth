using CIS.DataAccess;
using CIS.Domain.Customers.Models;
using CIS.Domain.Customers.Services;
using CIS.Library.Customers.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.UnitTesting.DataAccess.Customers.Services
{
    public sealed class CustomerServiceTests : DataAccessTest
    {
        [Test]
        public void ServicesAvailableInDI()
        {
            var customerService = Services.GetRequiredService<ICustomerService>();

            Assert.IsNotNull(customerService);
        }

        [Test]
        public async Task TestImportCustomers()
        {
            var customerService = Services.GetRequiredService<ICustomerService>();
            var dbContext = Services.GetRequiredService<CISDbContext>();
            var customersToImport = new List<CustomerImportDefinition>()
            {
                new CustomerImportDefinition()
                {
                    Number = 1,
                    Name = "Sebastian Nordby",
                    ContactPersonName = null,
                    ContactPersonEmailAddress = null,
                    ContactPersonPhoneNumber = null,
                    CustomerGroupNumber = null,
                    IsActive = true,
                },
                new CustomerImportDefinition()
                {
                    Number = 2,
                    Name = "Sebastian Bjornstad",
                    ContactPersonName = null,
                    ContactPersonEmailAddress = null,
                    ContactPersonPhoneNumber = null,
                    CustomerGroupNumber = null,
                    IsActive = true,
                    Store = new()
                    {
                        Number = 2,
                        Name = "Oslo Lufthavn",
                        AddressLine = "Lørenhagen 1234",
                        AddressPostalCode = "9876",
                        AddressPostalOffice = "Nordgata"
                    }
                }
            };

            var result = await customerService.Import(
                customersToImport);

            var customerCount = dbContext.Customers
                .AsNoTracking()
                .Count();

            var storeCount = dbContext.Stores
                .AsNoTracking()
                .Count();

            var storeCountShouldBeCreated = customersToImport
                .Where(x => x.Store is not null)
                .Count();

            Assert.IsTrue(result);
            Assert.AreEqual(customerCount, customersToImport.Count);
            Assert.AreEqual(storeCount, storeCountShouldBeCreated);
        }
    }
}
