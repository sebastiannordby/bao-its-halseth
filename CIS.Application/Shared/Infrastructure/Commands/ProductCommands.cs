using CIS.Application.Features.Products.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Infrastructure.Commands
{
    internal sealed class ProductCommands : IProductCommands
    {
        private readonly CISDbContext _dbContext;

        public ProductCommands(CISDbContext context)
        {
            _dbContext = context;
        }

        public async Task DeleteAllProductData(CancellationToken cancellationToken)
        {
            await _dbContext.ProductPrices.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Products.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.ProductGroups.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.StockCounts.ExecuteDeleteAsync(cancellationToken);
        }
    }
}
