using CIS.Application.Legacy;
using CIS.Application.Orders.Models;
using CIS.Library.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Repositories
{
    public interface ISalesOrderViewRepository
    {
        Task<IReadOnlyCollection<SalesOrderView>> List(int pageSize, int pageIndex);
        IQueryable<SalesOrderView> Query();
        Task<IReadOnlyCollection<MostSoldProductView>> GetMostSoldProduct(int count);
        Task<IReadOnlyCollection<StoreMostBoughtView>> GetMostBoughtViews(int count);
    }

    internal class SalesOrderViewRepository : ISalesOrderViewRepository
    {
        private readonly CISDbContext _dbContext;

        public SalesOrderViewRepository(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<StoreMostBoughtView>> GetMostBoughtViews(int count)
        {
            var query = (
                from sale in _dbContext.SalesStatistics
                    .AsNoTracking()
                join store in _dbContext.Stores
                    on sale.StoreNumber equals store.Number
                group new { sale, store } by
                    new { sale.StoreNumber, store.Name, sale.Quantity }
                    into productGroup
                orderby productGroup.Sum(x => x.sale.PurchasePrice) descending
                select new StoreMostBoughtView
                {
                    StoreName = productGroup.Key.Name,
                    TotalBoughtFor = productGroup.Sum(x => x.sale.PurchasePrice),
                    TotalCost = productGroup.Sum(x => x.sale.CostPrice),
                }
            ).Take(count);

            var list = await query.ToListAsync();

            return list;
        }

        public async Task<IReadOnlyCollection<MostSoldProductView>> GetMostSoldProduct(int count)
        {
            var query = (
                from sale in _dbContext.SalesStatistics
                    .AsNoTracking()
                join product in _dbContext.Products
                    on sale.ProductNumber equals product.Number
                group new { sale, product } by
                    new { sale.ProductNumber, product.Name }
                    into productGroup
                orderby productGroup.Sum(x => x.sale.Quantity) descending
                select new MostSoldProductView
                {
                    ProductNumber = productGroup.Key.ProductNumber,
                    ProductName = productGroup.Key.Name,
                    TotalQuantity = productGroup.Sum(x => x.sale.Quantity),
                    TotalSoldFor = productGroup.Sum(x => x.sale.PurchasePrice),
                    TotalCostPrice = productGroup.Sum(x => x.sale.CostPrice)
                }
            ).Take(count);

            var list = await query.ToListAsync();

            return list;
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

        public IQueryable<SalesOrderView> Query()
        {
            var orderQuery = (
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
            );

            return orderQuery;
        }
    }
}
