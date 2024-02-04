using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Library.Stores.Models;
using CIS.Library.Stores.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CIS.DataAccess.Stores.Repositories
{
    internal class StoreViewRepository : IStoreViewRepository
    {
        private readonly CISDbContext _dbContext;

        public StoreViewRepository(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<StoreView>> List()
        {
            var storesQuery = (
                from store in _dbContext.Stores
                    .AsNoTracking()
                join customer in _dbContext.Customers
                    on store.OwnerCustomerNumber equals customer.Number
                from customerGroup in _dbContext.CustomerGroups
                    .Where(x =>
                        customer.CustomerGroupNumber.HasValue &&
                        customer.CustomerGroupNumber == customer.CustomerGroupNumber)
                    .DefaultIfEmpty()

                select new StoreView
                {
                    Number = store.Number,
                    Name = store.Name,
                    IsActive = store.IsActive,
                    CustomerNumber = customer.Number,
                    CustomerName = customer.Name,
                    CustomerContactPersonName = customer.ContactPersonName,
                    CustomerContactPersonEmailAddress = customer.ContactPersonEmailAddress,
                    CustomerContactPersonPhoneNumber = customer.ContactPersonPhoneNumber,
                    CustomerCustomerGroupNumber = customerGroup != null ? customerGroup.Number : null,
                    CustomerGroupName = customerGroup != null ? customerGroup.Name : null
                }
            );

            var result = await storesQuery.ToListAsync();

            return result;
        }
    }
}
