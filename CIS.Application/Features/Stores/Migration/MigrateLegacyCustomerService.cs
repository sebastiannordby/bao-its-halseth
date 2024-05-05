using CIS.Application.Features.Stores.Import.Contracts;
using CIS.Application.Features.Stores.Migration.Contracts;
using CIS.Application.Features.Stores.Models.Import;
using CIS.Application.Legacy;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.Extensions.Logging;

namespace CIS.Application.Features.Stores.Migration
{
    internal class MigrateLegacyCustomerService :
        IMigrateLegacyService<Butikkliste>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _swnDistroContext;
        private readonly INotifyClientService _notifyClientService;
        private readonly IProcessImportCommandService<ImportCustomerCommand> _importService;
        private readonly IMigrationMapper<LegacySystemCustomerSource, ImportCustomerDefinition> _mapper;
        private readonly ILogger<MigrateLegacyCustomerService> _logger;

        public MigrateLegacyCustomerService(
            CISDbContext dbContext,
            SWNDistroContext swnDistroContext,
            INotifyClientService notifyClientService,
            IProcessImportCommandService<ImportCustomerCommand> importService,
            IMigrationMapper<LegacySystemCustomerSource, ImportCustomerDefinition> mapper,
            ILogger<MigrateLegacyCustomerService> logger)
        {
            _dbContext = dbContext;
            _swnDistroContext = swnDistroContext;
            _notifyClientService = notifyClientService;
            _importService = importService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Migrate(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Migrating customers and stores from SWNDistro to CIS started.");
            await _notifyClientService.SendPlainText("Importering av kunder/butikker påbegynt.");

            await _swnDistroContext.Butikklistes.ProcessEntitiesInBatches(async (customers, percentage) =>
            {
                var importDefinitions = await _mapper.Map(new()
                {
                    Data = customers
                }, cancellationToken);

                var success = await _importService.Import(new()
                {
                    Definitions = importDefinitions
                }, cancellationToken);

                var names = importDefinitions
                    .Select(x => x.Name)
                    .ToArray();

                var namesMsg = string.Join("\n", names);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({percentage}%)({successMsg}) Kunder:\n{namesMsg}\n";

                await _notifyClientService.SendPlainText(message);
            });

            await _notifyClientService.SendPlainText("Importering av kunder/butikker vellykket.");
            _logger.LogInformation("Migrating customers and stores from SWNDistro to CIS ended successfully.");
        }
    }
}
