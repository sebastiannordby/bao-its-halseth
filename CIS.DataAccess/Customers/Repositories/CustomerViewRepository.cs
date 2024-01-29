﻿using CIS.Library.Customers.Models;
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
            var customerList = await _dbContext.Customers
                .AsNoTracking()
                .Select(x => new CustomerView(
                    x.Number,
                    x.Name,
                    x.ContactPersonName,
                    x.ContactPersonEmailAddress,
                    x.ContactPersonPhoneNumber,
                    x.CustomerGroupNumber,
                    null,
                    x.IsActive
                )).ToListAsync();

            return customerList.AsReadOnly();
        }
    }
}