using CIS.Library.Orders.Models;
using CIS.Library.Orders.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Orders.Repositories
{
    internal class SalesOrderViewRepository : ISalesOrderViewRepository
    {
        private readonly CISDbContext _dbContext;

        public SalesOrderViewRepository(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<SalesOrderView>> List(int pageSize, int pageIndex)
        {
            var orderList = await (
                from order in _dbContext.SalesOrders
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)

                select new SalesOrderView()
                {
                    Id = order.Id,
                    Number = order.Number,
                    AlternateNumber = order.AlternateNumber,
                    OrderDate = order.OrderDate,
                    Reference = order.Reference,
                    DeliveredDate = order.DeliveredDate,
                    StoreNumber = order.StoreNumber,
                    StoreName = order.StoreName,
                    CustomerNumber = order.CustomerNumber,
                    CustomerName = order.CustomerName,
                    IsDeleted = order.IsDeleted,
                }
            ).Skip(pageSize * pageIndex)
             .Take(pageSize)
             .ToListAsync();

            return orderList.AsReadOnly();
        }
    }
}
