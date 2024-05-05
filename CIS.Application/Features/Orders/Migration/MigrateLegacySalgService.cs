using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Import.Contracts;
using CIS.Application.Features.Orders.Migration.Contracts;
using CIS.Application.Legacy;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;

namespace CIS.Application.Features.Orders.Migration
{
    internal class MigrateLegacySalgService : IMigrateLegacyService<Salg>
    {
        private readonly SWNDistroContext _swnDbContext;
        private readonly INotifyClientService _notifyClient;
        private readonly IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition> _migrationMapper;
        private readonly IProcessImportCommandService<ImportSalesStatisticsCommand> _importService;

        public MigrateLegacySalgService(
            SWNDistroContext swnDbContext,
            INotifyClientService notifyClientService,
            IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition> migrationMapper,
            IProcessImportCommandService<ImportSalesStatisticsCommand> importService)
        {
            _swnDbContext = swnDbContext;
            _notifyClient = notifyClientService;
            _importService = importService;
            _migrationMapper = migrationMapper;
            _importService = importService;
        }

        public async Task Migrate(CancellationToken cancellationToken)
        {
            await _notifyClient.SendPlainText("Importering av salgstall påbegynt.");

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

                await _notifyClient.SendPlainText(message);
            }, 1500);

            await _notifyClient.SendPlainText("Importering av salgstall vellykket.");
        }
    }
}
