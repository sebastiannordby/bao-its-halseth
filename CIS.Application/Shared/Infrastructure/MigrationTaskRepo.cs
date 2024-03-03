﻿using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using ShopifySharp.GraphQL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
            //todo: fix så det ikke kræasjer, blir kalt i både navmenu og admin/home samtidig

             return await _dbContext.MigrationsTasks
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        public async Task<bool> HasMigrated(CancellationToken cancellationToken)
        {
            var migTask = await GetMigrationTasks(cancellationToken);
            return migTask.Where(s => !s.Executed).Any();
        }

        public async Task<bool> IsAllMigrationsExecuted(CancellationToken cancellationToken)
        {
            var result = await _dbContext.MigrationsTasks
                .AnyAsync(x => !x.Executed, cancellationToken);

            return !result;
        }
    }
}
