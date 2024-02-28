using CIS.Application.Features.Stores.Infrastructure;
using CIS.Application.Features.Stores.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Infrastructure
{
    internal sealed class CustomerQueries : IStoreQueries
    {
        private readonly CISDbContext _dbContext;

        public CustomerQueries(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<CustomerView>> List(
            CancellationToken cancellationToken = default)
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
                    Id = customer.Id,
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

            return customerList;
        }
    }
}
