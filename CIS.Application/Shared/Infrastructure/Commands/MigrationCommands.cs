using CIS.Application.Features.Orders.Migration.Infrastructure;
using CIS.Application.Shared.Models;
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
            await _dbContext.MigrationsTasks
                .ExecuteDeleteAsync(cancellationToken);

            var migrationTaskTypes = Enum.GetValues<MigrationTask.TaskType>();

            foreach (var type in migrationTaskTypes)
            {
                await _dbContext.MigrationsTasks.AddAsync(new()
                {
                    Type = type,
                    Executed = false
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
