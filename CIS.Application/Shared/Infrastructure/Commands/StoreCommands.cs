using CIS.Application.Features.Stores.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Infrastructure.Commands
{
    internal sealed class StoreCommands : IStoreCommands
    {
        private readonly CISDbContext _dbContext;

        public StoreCommands(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteAllStoreData(CancellationToken cancellationToken)
        {
            await _dbContext.Stores.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Customers.ExecuteDeleteAsync(cancellationToken);
            await _dbContext.Stores.ExecuteDeleteAsync(cancellationToken);
        }
    }
}
