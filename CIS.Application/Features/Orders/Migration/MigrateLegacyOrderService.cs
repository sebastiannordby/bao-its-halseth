using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Import.Contracts;
using CIS.Application.Features.Orders.Migration.Contracts;
using CIS.Application.Legacy;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CIS.Application.Features.Orders.Migration
{
    internal class MigrateLegacyOrderService : IMigrateLegacyService<Ordre>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _swnDbContext;
        private readonly IProcessImportCommandService<ImportSalesOrderCommand> _processImportService;
        private readonly IMigrationMapper<LegacySystemSalesOrderSource, ImportSalesOrderDefinition> _migrationMapper;
        private readonly INotifyClientService _notifyClientService;
        private readonly ILogger<MigrateLegacyOrderService> _logger;

        public MigrateLegacyOrderService(
            CISDbContext dbContext,
            SWNDistroContext swnDbContext,
            IProcessImportCommandService<ImportSalesOrderCommand> processImportService,
            IMigrationMapper<LegacySystemSalesOrderSource, ImportSalesOrderDefinition> migrationMapper,
            INotifyClientService notifyClientService,
            ILogger<MigrateLegacyOrderService> logger)
        {
            _dbContext = dbContext;
            _swnDbContext = swnDbContext;
            _processImportService = processImportService;
            _migrationMapper = migrationMapper;
            _notifyClientService = notifyClientService;
            _logger = logger;
        }

        public async Task Migrate(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Migrating orders from SWNDistro to CIS started.");
            await _notifyClientService.SendPlainText("Migrering av ordre/bestillinger påbegynt.");

            var allOrderGroupingsQuery = _swnDbContext.Ordres
                .AsNoTracking()
                .GroupBy(x => new { x.Dato, x.Butikknr })
                .Select(x => new OrderGroupingStruct
                {
                    Dato = x.Key.Dato,
                    Butikknr = x.Key.Butikknr
                });

            await allOrderGroupingsQuery.ProcessEntitiesInBatches(async (batchGroup, percentageDone) =>
            {
                var orderGroupAggregate = (
                    from orderGrouping in batchGroup
                    join orders in _swnDbContext.Ordres
                        on new { orderGrouping.Butikknr, orderGrouping.Dato } equals new { orders.Butikknr, orders.Dato }
                        into orderGroup
                    select new
                    {
                        OrderGrouping = orderGrouping,
                        Orders = orderGroup
                    }
                ).Select(x => x.Orders).ToList();


                var importDefinitions = await _migrationMapper.Map(new LegacySystemSalesOrderSource()
                {
                    OrderGrouping = orderGroupAggregate
                }, cancellationToken);

                var success = await _processImportService.Import(new()
                {
                    Definitions = importDefinitions
                }, cancellationToken);

                var textLines = importDefinitions
                    .Select(x => x.Number.ToString())
                    .ToArray();

                var text = string.Join("\n", textLines);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({percentageDone}%)({successMsg}) Ordre:\n{text}\n";
                await _notifyClientService.SendPlainText(message);
            });

            await _notifyClientService.SendPlainText("Importering av ordre/bestillinger vellykket.");
            _logger.LogInformation("Migrating orders from SWNDistro to CIS ended successfully.");
        }

        private class OrderGroupingStruct
        {
            public DateTime? Dato { get; set; }
            public int? Butikknr { get; set; }
        }
    }
}
