using CIS.Application.Features.Stores.Models;
using CIS.Application.Features.Stores.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Stores.Services.Implementation
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
            string countedByPersonFullName,
            CancellationToken cancellationToken)
        {
            var stockCount = new StockCountDao()
            {
                ProductId = productId,
                StoreId = storeId,
                Quantity = quantity,
                CountedByPersonFullName = countedByPersonFullName,
                CountedDateTime = DateTime.Now
            };

            await _dbContext.StockCounts.AddAsync(stockCount, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<StockCountView>> GetByStore(
            Guid storeId, CancellationToken cancellationToken)
        {
            var newestStockCounts = await _dbContext.StockCounts
                .AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .GroupBy(x => x.ProductId)
                .Select(g => g.OrderByDescending(x => x.CountedDateTime).First())
                .ToListAsync(cancellationToken);

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

        public async Task<IReadOnlyCollection<StockCountView>> GetHistoryByStore(
            Guid storeId, CancellationToken cancellationToken)
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
            ).OrderByDescending(x => x.CountedDateTime).ToListAsync(cancellationToken);

            return stockCountViews;
        }
    }
}
