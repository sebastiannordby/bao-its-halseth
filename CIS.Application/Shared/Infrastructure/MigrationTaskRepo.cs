using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Infrastructure
{
    internal class MigrationTaskRepo : IMigrationTaskRepo
    {
        private readonly CISDbContext _dbContext;

        public MigrationTaskRepo(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Complete(
            MigrationTask.TaskType taskType, CancellationToken cancellationToken)
        {
            await _dbContext.MigrationsTasks
                .Where(x => x.Type == taskType)
                .ExecuteUpdateAsync(x =>
                    x.SetProperty(x => x.Executed, true), cancellationToken);
        }

        public async Task<IEnumerable<MigrationTask>> GetMigrationTasks(CancellationToken cancellationToken)
        {
            var migrationTasks = await _dbContext.MigrationsTasks
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return migrationTasks;
        }
    }
}
