using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Application.Features.Stores.Models;
using CIS.Application.Features.Stores.Services;
using Microsoft.EntityFrameworkCore;

namespace CIS.Application.Features.Stores.Services.Implementation
{
    internal class StoreService : IStoreService
    {
        private readonly CISDbContext _dbContext;

        public StoreService(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<StoreView>> List(CancellationToken cancellationToken)
        {
            var storesList = await (
                from store in _dbContext.Stores
                    .AsNoTracking()
                join customer in _dbContext.Customers
                    on store.OwnerCustomerId equals customer.Id
                from customerGroup in _dbContext.CustomerGroups
                    .Where(x =>
                        customer.CustomerGroupId.HasValue &&
                        customer.CustomerGroupId == customer.CustomerGroupId)
                    .DefaultIfEmpty()

                select new StoreView
                {
                    Number = store.Number,
                    Name = store.Name,
                    IsActive = store.IsActive,
                    AddressLine = store.AddressLine,
                    PostalOffice = store.AddressPostalOffice,
                    PostalCode = store.AddressPostalCode,
                    CustomerNumber = customer.Number,
                    CustomerName = customer.Name,
                    CustomerContactPersonName = customer.ContactPersonName,
                    CustomerContactPersonEmailAddress = customer.ContactPersonEmailAddress,
                    CustomerContactPersonPhoneNumber = customer.ContactPersonPhoneNumber,
                    CustomerGroupNumber = customerGroup != null ?
                        customerGroup.Number : null,
                    CustomerGroupName = customerGroup != null ?
                        customerGroup.Name : null,
                }
            ).ToListAsync(cancellationToken);

            return storesList.AsReadOnly();
        }
    }
}
