using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Orders.Contracts.Import;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using CIS.Application.Shared.Services;
using CIS.Library.Customers.Models.Import;
using CIS.Library.Products.Import;
using CIS.Library.Shared.Services;
using CIS.WebApp.Components.Pages;
using ExcelDataReader.Log;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CIS.WebApp.Services 
{
    public class ImportLegacyDataBackgroundService
    {
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        private const string ReceiveMessage = "ReceiveMessage";
        private const string Finished = "Finished";

        public ImportLegacyDataBackgroundService(
            IServiceScopeFactory scopeFactory,
            IHubContext<ImportLegacyDataHub, IListenImportClient> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_isRunning)
                return;

            _isRunning = true;
            using var scope = _scopeFactory.CreateScope();

            var migrateSalesStatisticsService = scope.ServiceProvider.GetRequiredService<IMigrateLegacyService<Salg>>();
            var migrateSalesOrderService = scope.ServiceProvider.GetRequiredService<IMigrateLegacyService<Ordre>>();
            var migrateCustomerService = scope.ServiceProvider.GetRequiredService<IMigrateLegacyService<Butikkliste>>();
            var migrateProductService = scope.ServiceProvider.GetRequiredService<IMigrateLegacyService<Vareinfo>>();

            var legacyDbContext = scope.ServiceProvider
                .GetRequiredService<SWNDistroContext>();

            var migrationTaskRepo = scope.ServiceProvider
                .GetRequiredService<IMigrationTaskRepo>();

            var tasks = await migrationTaskRepo.GetMigrationTasks();
            var uncompletedTasks = tasks.Where(x => !x.Executed);

            if(uncompletedTasks.Any(x => x.Type == MigrationTask.TaskType.Products))
            {
                await migrateProductService.Migrate();
                await migrationTaskRepo.Complete(MigrationTask.TaskType.Products);
            }

            if (uncompletedTasks.Any(x => x.Type == MigrationTask.TaskType.Customers))
            {
                await migrateCustomerService.Migrate();
                await migrationTaskRepo.Complete(MigrationTask.TaskType.Customers);
            }

            if (uncompletedTasks.Any(x => x.Type == MigrationTask.TaskType.SalesOrders))
            {
                await migrateSalesOrderService.Migrate();
                await migrationTaskRepo.Complete(MigrationTask.TaskType.SalesOrders);
            }

            if (uncompletedTasks.Any(x => x.Type == MigrationTask.TaskType.SalesOrderStatistics))
            {
                await migrateSalesStatisticsService.Migrate();
                await migrationTaskRepo.Complete(MigrationTask.TaskType.SalesOrderStatistics);
            }

            await _hubContext.Clients.All.Finished();

            _isRunning = false;
        }
    }
}