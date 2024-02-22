using CIS.Application.Stores.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Stores.Services.Implementation
{
    internal sealed class StockCountService : IStockCountService
    {
        private readonly CISDbContext _dbContext;

        public StockCountService(CISDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddStockCount(
            Guid productId, 
            Guid storeId, 
            int quantity,
            string countedByPersonFullName)
        {
            var stockCount = new StockCountDao()
            {
                ProductId = productId,
                StoreId = storeId,
                Quantity = quantity,
                CountedByPersonFullName = countedByPersonFullName,
                CountedDateTime = DateTime.Now
            };

            await _dbContext.StockCounts.AddAsync(stockCount);
            await _dbContext.SaveChangesAsync();
        }

        public Task AddStockCount(Guid productId, Guid storeId, int quantity)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<StockCountView>> GetByStore(Guid storeId)
        {
            var newestStockCounts = await _dbContext.StockCounts
                .AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .GroupBy(x => x.ProductId)
                .Select(g => g.OrderByDescending(x => x.CountedDateTime).First())
                .ToListAsync();

            var stockCountViews = (
                from newestStockCount in newestStockCounts
                join product in _dbContext.Products
                    on newestStockCount.ProductId equals product.Id
                select new StockCountView()
                {
                    ProductNumber = product.Number,
                    ProductName = product.Name,
                    Quantity = newestStockCount.Quantity,
                    CountedByPersonFullName = newestStockCount.CountedByPersonFullName,
                    CountedDateTime = newestStockCount.CountedDateTime
                }
            ).ToList();

            return stockCountViews;
        }

        public async Task<IReadOnlyCollection<StockCountView>> GetHistoryByStore(Guid storeId)
        {
            var stockCountViews = await (
                from stockCount in _dbContext.StockCounts
                    .AsNoTracking()
                    .Where(x => x.StoreId == storeId)
                join product in _dbContext.Products
                    on stockCount.ProductId equals product.Id
                select new StockCountView()
                {
                    ProductNumber = product.Number,
                    ProductName = product.Name,
                    Quantity = stockCount.Quantity,
                    CountedByPersonFullName = stockCount.CountedByPersonFullName,
                    CountedDateTime = stockCount.CountedDateTime
                }
            ).OrderByDescending(x => x.CountedDateTime).ToListAsync();

            return stockCountViews;
        }
    }
}
