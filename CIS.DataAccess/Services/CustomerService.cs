using CIS.DataAccess.Models;
using CIS.Domain.Customers.Models;
using CIS.Domain.Customers.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Services
{
    internal class CustomerService : ICustomerService
    {
        private readonly CISDbContext _dbContext;

        public CustomerService(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Import(IEnumerable<CustomerImportDefinition> importDefinitions)
        {
            foreach(var customer in importDefinitions)
            {
                var doesExist = await _dbContext.Customers
                    .AnyAsync(x => x.Number == customer.Number);
                if (doesExist)
                    continue;

                var customerDao = new CustomerDao()
                {
                    Number = customer.Number,
                    Name = customer.Name,
                    ContactPersonEmailAddress = customer.ContactPersonEmailAddress,
                    ContactPersonName = customer.ContactPersonName,
                    ContactPersonPhoneNumber = customer.ContactPersonPhoneNumber,
                    CustomerGroupNumber = customer.CustomerGroupNumber,
                    IsActive = customer.IsActive
                };

                await _dbContext.Customers.AddAsync(customerDao);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }
    }
}
