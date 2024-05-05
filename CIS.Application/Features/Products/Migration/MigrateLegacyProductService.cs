using CIS.Application.Features.Products.Import.Contracts;
using CIS.Application.Features.Products.Migration.Contracts;
using CIS.Application.Features.Products.Models.Import;
using CIS.Application.Legacy;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.Extensions.Logging;

namespace CIS.Application.Features.Products.Migration
{
    internal class MigrateLegacyProductService : IMigrateLegacyService<Vareinfo>
    {
        private readonly SWNDistroContext _swnDistroContext;
        private readonly IProcessImportCommandService<ImportProductCommand> _importService;
        private readonly INotifyClientService _notifyClientService;
        private readonly IMigrationMapper<LegacySystemProductSource, ImportProductDefinition> _mapper;
        private readonly ILogger<MigrateLegacyProductService> _logger;

        public MigrateLegacyProductService(
            SWNDistroContext swnDistroContext,
            INotifyClientService notifyClientService,
            IProcessImportCommandService<ImportProductCommand> importService,
            IMigrationMapper<LegacySystemProductSource, ImportProductDefinition> mapper,
            ILogger<MigrateLegacyProductService> logger)
        {
            _swnDistroContext = swnDistroContext;
            _notifyClientService = notifyClientService;
            _importService = importService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Migrate(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Migrating products from SWNDistro to CIS started.");
            await _notifyClientService.SendPlainText("Importering av varer påbegynt.");

            await _swnDistroContext.Vareinfos.ProcessEntitiesInBatches(async (products, percentage) =>
            {
                var importDefinitions = await _mapper.Map(new()
                {
                    Data = products
                }, cancellationToken);

                var success = await _importService.Import(new()
                {
                    Definitions = importDefinitions
                }, cancellationToken);

                var productNames = importDefinitions
                    .Select(x => x.Name)
                    .ToArray();

                var productNamesMsg = string.Join("\n", productNames);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({percentage}%)({successMsg}) Varer:\n{productNamesMsg}\n";

                await _notifyClientService.SendPlainText(message);
            });

            await _notifyClientService.SendPlainText("Importering av varer vellykket.");
            _logger.LogInformation("Migrating products from SWNDistro to CIS ended successfully.");
        }
    }
}
