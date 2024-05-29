using CIS.Application.Features.Orders.Migration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Infrastructure.Commands
{
    internal class MigrationCommands : IMigrationCommands
    {
        private readonly CISDbContext _dbContext;

        public MigrationCommands(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteAllMigrationData(CancellationToken cancellationToken)
        {
            await _dbContext.MigrationsTasks.ExecuteDeleteAsync(cancellationToken);
        }
    }
}
