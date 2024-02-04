using CIS.Library.Customers.Models;
using CIS.Library.Customers.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Customers.Repositories
{
    internal class CustomerViewRepository : ICustomerViewRepository
    {
        private readonly CISDbContext _dbContext;

        public CustomerViewRepository(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<CustomerView>> List()
        {
            var customerList = await (
                from customer in _dbContext.Customers
                    .AsNoTracking()
                from customerGroup in _dbContext.CustomerGroups
                    .Where(x => 
                        customer.CustomerGroupId.HasValue && 
                        customer.CustomerGroupId.Value == x.Id)
                    .DefaultIfEmpty()

                select new CustomerView()
                {
                    Number = customer.Number,
                    Name = customer.Name,
                    ContactPersonName = customer.ContactPersonName,
                    ContactPersonEmailAddress = customer.ContactPersonEmailAddress,
                    ContactPersonPhoneNumber = customer.ContactPersonPhoneNumber,
                    IsActive = customer.IsActive,
                    CustomerGroupId = customerGroup != null ? 
                        customerGroup.Id : null,
                    CustomerGroupNumber = customerGroup != null ? 
                        customerGroup.Number : null,
                    CustomerGroupName = customerGroup != null ? 
                        customerGroup.Name : null,
                }
            ).ToListAsync();

            return customerList.AsReadOnly();
        }
    }
}
