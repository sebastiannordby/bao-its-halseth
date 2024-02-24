using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Orders.Contracts.Import;
using CIS.Application.Orders.Import.Contracts;
using CIS.Application.Orders.Migration.Contracts;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Migration
{
    internal class MigrateLegacySalgService : IMigrateLegacyService<Salg>
    {
        private readonly SWNDistroContext _swnDbContext;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hub;
        private readonly IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition> _migrationMapper;
        private readonly IProcessImportCommandService<ImportSalesStatisticsCommand> _importService;

        public MigrateLegacySalgService(
            SWNDistroContext swnDbContext,
            IHubContext<ImportLegacyDataHub, IListenImportClient> hub,
            IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition> migrationMapper,
            IProcessImportCommandService<ImportSalesStatisticsCommand> importService)
        {
            _swnDbContext = swnDbContext;
            _hub = hub;
            _importService = importService;
            _migrationMapper = migrationMapper;
            _importService = importService;
        }

        public async Task Migrate(CancellationToken cancellationToken)
        {
            await _hub.Clients.All.ReceiveMessage("Importering av salgstall påbegynt.");

            await _swnDbContext.Salgs.ProcessEntitiesInBatches(async (sales, percentage) =>
            {
                var importDefinitions = await _migrationMapper.Map(new LegacySystemSalesStatisticsSource()
                {
                    Data = sales
                }, cancellationToken);

                var success = await _importService.Import(new()
                {
                    Definitions = importDefinitions
                }, CancellationToken.None);

                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({percentage}%)({successMsg}) Salgstall..\n";

                await _hub.Clients.All.ReceiveMessage(message);
            }, 1500);

            await _hub.Clients.All.ReceiveMessage("Importering av salgstall vellykket.");
        }
    }
}
