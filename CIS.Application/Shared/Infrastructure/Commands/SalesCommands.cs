using CIS.Application.Features.Orders.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Infrastructure.Commands
{
    internal class SalesCommands : ISalesCommands
    {
        private readonly CISDbContext _dbContext;

        public SalesCommands(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteAllSalesData(CancellationToken cancellationToken)
        {
            await _dbContext.SalesOrderLines.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.SalesOrders.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.SalesStatistics.ExecuteDeleteAsync(cancellationToken);
        }
    }
}
