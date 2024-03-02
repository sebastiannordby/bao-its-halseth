using CIS.Application.Features.Stores.Import.Contracts;
using CIS.Application.Features.Stores.Migration.Contracts;
using CIS.Application.Features.Stores.Models.Import;
using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Stores.Migration
{
    internal class MigrateLegacyCustomerService :
        IMigrateLegacyService<Butikkliste>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _swnDistroContext;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hub;
        private readonly IProcessImportCommandService<ImportCustomerCommand> _importService;
        private readonly IMigrationMapper<LegacySystemCustomerSource, ImportCustomerDefinition> _mapper;

        public MigrateLegacyCustomerService(
            CISDbContext dbContext,
            SWNDistroContext swnDistroContext,
            IHubContext<ImportLegacyDataHub, IListenImportClient> hub,
            IProcessImportCommandService<ImportCustomerCommand> importService,
            IMigrationMapper<LegacySystemCustomerSource, ImportCustomerDefinition> mapper)
        {
            _dbContext = dbContext;
            _swnDistroContext = swnDistroContext;
            _hub = hub;
            _importService = importService;
            _mapper = mapper;
        }

        public async Task Migrate(CancellationToken cancellationToken)
        {
            await _hub.Clients.All.ReceiveMessage("Importering av kunder/butikker påbegynt.");

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

                await _hub.Clients.All.ReceiveMessage(message);
            });
        }

    }
}
