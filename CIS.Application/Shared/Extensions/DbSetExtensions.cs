using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Extensions
{
    public static class DbSetExtensions
    {
        public static async Task ProcessEntitiesInBatches<T>(
            this DbSet<T> dbSet,
            Func<IReadOnlyCollection<T>, int, Task> processBatch,
            int batchSize = 50)
            where T : class
        {
            var totalRecords = await dbSet.CountAsync();
            var offset = 0;

            while (true)
            {
                // Query for the next batch of entities
                var batch = await dbSet
                    .AsNoTracking()
                    .Skip(offset)
                    .Take(batchSize)
                    .ToListAsync();

                var batchCount = batch.Count;
                var numberOfRecordsProcessed = offset + batchCount;
                var newPercentage = (int)Math.Round(
                    (double)numberOfRecordsProcessed / totalRecords * 100);

                if (batch.Count == 0)
                    break; // No more entities to process

                // Process the batch
                await processBatch(batch, newPercentage);

                offset += batch.Count;
            }
        }

        public static async Task ProcessEntitiesInBatches<T>(
            this IQueryable<T> dbSet,
            Func<IEnumerable<T>, int, Task> processBatch)
            where T : class
        {
            var totalRecords = await dbSet.CountAsync();
            var batchSize = 100;
            var offset = 0;

            while (offset < totalRecords)
            {
                // Query for the next batch of entities
                var batch = await dbSet
                    .AsNoTracking()
                    .Skip(offset)
                    .Take(batchSize)
                    .ToListAsync();

                // Process the batch
                await processBatch(batch, CalculatePercentage(offset, totalRecords));

                offset += batch.Count;
            }
        }

        private static int CalculatePercentage(int offset, int totalRecords)
        {
            return (int)Math.Round((double)(offset * 100) / totalRecords);
        }
    }
}
