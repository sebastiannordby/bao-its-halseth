using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Orders.Contracts;
using CIS.Application.Orders.Import.Contracts;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Application.Shopify;
using CIS.Library.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Services
{
    internal class MigrateLegacyOrderService : IMigrateLegacyService<Ordre>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _swnDbContext;
        private readonly IProcessImportCommandService<ImportSalesOrderCommand> _processImportService;
        private readonly IMigrationMapper<IEnumerable<OrderGroupingStruct>, IEnumerable<SalesOrderImportDefinition>> _migrationMapper;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hub;

        public MigrateLegacyOrderService(
            CISDbContext dbContext,
            SWNDistroContext swnDbContext,
            IProcessImportCommandService<ImportSalesOrderCommand> processImportService,
            IMigrationMapper<IEnumerable<OrderGroupingStruct>, IEnumerable<SalesOrderImportDefinition>> migrationMapper,
            IHubContext<ImportLegacyDataHub, IListenImportClient> hub)
        {
            _dbContext = dbContext;
            _swnDbContext = swnDbContext;
            _processImportService = processImportService;
            _migrationMapper = migrationMapper;
            _hub = hub;
        }

        public async Task Migrate()
        {
            await _hub.Clients.All.ReceiveMessage("Importering av ordre/bestillinger påbegynt.");

            var allOrderGroupingsQuery = _swnDbContext.Ordres
                .AsNoTracking()
                .GroupBy(x => new { x.Dato, x.Butikknr })
                .Select(x => new OrderGroupingStruct
                {
                    Dato = x.Key.Dato,
                    Butikknr = x.Key.Butikknr
                });

            await allOrderGroupingsQuery.ProcessEntitiesInBatches(async (orderGroupings, percentageDone) =>
            {
                var importDefinitions = await _migrationMapper.Map(orderGroupings);
                var success = await _processImportService.Import(new()
                {
                    Definitions = importDefinitions
                }, CancellationToken.None);

                var textLines = importDefinitions
                    .Select(x => x.Number.ToString())
                    .ToArray();

                var text = string.Join("\n", textLines);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({percentageDone}%)({successMsg}) Ordre:\n{text}\n";
                await _hub.Clients.All.ReceiveMessage(message);
            });

            await _hub.Clients.All.ReceiveMessage("Importering av ordre/bestillinger vellykket.");
        }
    }
}
