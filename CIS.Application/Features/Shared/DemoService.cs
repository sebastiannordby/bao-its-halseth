using CIS.Application.Features.Orders.Infrastructure;
using CIS.Application.Features.Orders.Migration.Infrastructure;
using CIS.Application.Features.Products.Infrastructure;
using CIS.Application.Features.Stores.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Shared
{
    public sealed class DemoService
    {
        private readonly ISalesCommands _salesCommands;
        private readonly IMigrationCommands _migrationCommands;
        private readonly IProductCommands _productCommands;
        private readonly IStoreCommands _storeCommands;
        private readonly CISDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public DemoService(
            ISalesCommands salesCommands,
            IMigrationCommands migrationCommands,
            IProductCommands productCommands,
            IStoreCommands storeCommands,
            CISDbContext dbContext,
            UserManager<ApplicationUser> userManager)
        {
            _salesCommands = salesCommands;
            _migrationCommands = migrationCommands;
            _productCommands = productCommands;
            _storeCommands = storeCommands;
            _dbContext = dbContext;
            _userManager = userManager; 
        }

        public async Task ResetAllData(CancellationToken cancellationToken)
        {
            await _salesCommands.DeleteAllSalesData(cancellationToken);
            await _migrationCommands.DeleteAllMigrationData(cancellationToken);
            await _productCommands.DeleteAllProductData(cancellationToken);
            await _storeCommands.DeleteAllStoreData(cancellationToken);
        }
        
        public async Task EnsureCustomerWithMostOrderHasUser()
        {
            if(!_dbContext.Users.Any(x => x.UserName == DemoConstants.CUSTOMER_USERNAME))
            {
                var customerWithMostOrders = (
                        from order in _dbContext.SalesOrders
                        join customer in _dbContext.Customers
                            on order.CustomerNumber equals customer.Number
                        group order by order.CustomerNumber into groupedOrders
                        orderby groupedOrders.Count() descending
                        select new
                        {
                            CustomerId = groupedOrders.Key,
                            OrderCount = groupedOrders.Count()
                        }
                    ).FirstOrDefault();

                var findCustomer = customerWithMostOrders != null ?
                    _dbContext.Customers
                    .First(x => x.Number == customerWithMostOrders.CustomerId) : null;

                var user = new ApplicationUser
                {
                    UserName = DemoConstants.CUSTOMER_USERNAME,
                    Email = DemoConstants.CUSTOMER_PASSWORD,
                    LockoutEnabled = false,
                    TwoFactorEnabled = false,
                    CustomerId = findCustomer?.Id
                };
                var result = await _userManager.CreateAsync(user, DemoConstants.CUSTOMER_PASSWORD);
                var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmEmailRes = await _userManager.ConfirmEmailAsync(user, emailConfirmationCode);

                var role = new IdentityUserRole<string>()
                {
                    RoleId = UserTypes.CUSTOMER,
                    UserId = user.Id
                };

                await _dbContext.UserRoles.AddAsync(role);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
