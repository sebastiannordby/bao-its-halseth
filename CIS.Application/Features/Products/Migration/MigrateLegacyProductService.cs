using CIS.Application.Features.Products.Import.Contracts;
using CIS.Application.Features.Products.Migration.Contracts;
using CIS.Application.Features.Products.Models.Import;
using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Products.Migration
{
    internal class MigrateLegacyProductService : IMigrateLegacyService<Vareinfo>
    {
        private readonly SWNDistroContext _swnDistroContext;
        private readonly IProcessImportCommandService<ImportProductCommand> _importService;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hub;
        private readonly IMigrationMapper<LegacySystemProductSource, ImportProductDefinition> _mapper;
        private readonly ILogger<MigrateLegacyProductService> _logger;

        public MigrateLegacyProductService(
            SWNDistroContext swnDistroContext,
            IHubContext<ImportLegacyDataHub, IListenImportClient> hub,
            IProcessImportCommandService<ImportProductCommand> importService,
            IMigrationMapper<LegacySystemProductSource, ImportProductDefinition> mapper,
            ILogger<MigrateLegacyProductService> logger)
        {
            _swnDistroContext = swnDistroContext;
            _hub = hub;
            _importService = importService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Migrate(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Migrating products from SWNDistro to CIS started.");
            await _hub.Clients.All.ReceiveMessage("Importering av varer påbegynt.");

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

                await _hub.Clients.All.ReceiveMessage(message);
            });

            await _hub.Clients.All.ReceiveMessage("Importering av varer vellykket.");
            _logger.LogInformation("Migrating products from SWNDistro to CIS ended successfully.");
        }
    }
}
