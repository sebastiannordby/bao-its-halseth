using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Orders.Contracts;
using CIS.Application.Orders.Contracts.Import;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Services
{
    internal class ImportSalesStatisticsService : 
        IExecuteImportService<SalesStatisticsImportDefinition>,
        IMigrateLegacyService<Salg>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _swnDbContext;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hub;

        public ImportSalesStatisticsService(
            CISDbContext dbContext,
            SWNDistroContext swnDbContext,
            IHubContext<ImportLegacyDataHub, IListenImportClient> hub)
        {
            _dbContext = dbContext;
            _swnDbContext = swnDbContext;
            _hub = hub;
        }

        public async Task Migrate()
        {
            await _hub.Clients.All.ReceiveMessage("Importering av salgstall påbegynt.");

            await _swnDbContext.Salgs.ProcessEntitiesInBatches(async (sales, percentage) =>
            {
                var importDefinitions = new List<SalesStatisticsImportDefinition>();

                foreach (var sale in sales)
                {
                    var definition = new SalesStatisticsImportDefinition()
                    {
                        Number = (int)sale.Id,
                        Date = sale.Dato.Value,
                        ProductNumber = sale.VareId ?? 0,
                        CostPrice = sale.OurPrice ?? 0,
                        PurchasePrice = sale.Innpris ?? 0,
                        Quantity = sale.Antall ?? 0,
                        StoreNumber = sale.Butikknr ?? 0,
                        StorePrice = sale.Utpris ?? 0,
                        CustomerNumber = sale.Kundenr ?? 0,
                    };

                    importDefinitions.Add(definition);
                }

                var success = await Import(importDefinitions);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({percentage}%)({successMsg}) Salgstall..\n";

                await _hub.Clients.All.ReceiveMessage(message); 
            }, 500);

            await _hub.Clients.All.ReceiveMessage("Importering av salgstall vellykket.");
        }

        public async Task<bool> Import(IEnumerable<SalesStatisticsImportDefinition> definitions)
        {
            var statistics = new List<SalesStatisticsDao>();

            foreach(var definition in definitions) 
            {
                statistics.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Number = definition.Number,
                    Date = definition.Date,
                    CostPrice = definition.CostPrice,
                    ProductNumber = definition.ProductNumber,
                    CustomerNumber = definition.CustomerNumber,
                    PurchasePrice = definition.PurchasePrice,
                    Quantity = definition.Quantity,
                    StoreNumber = definition.StoreNumber,
                    StorePrice = definition.StorePrice,
                });
            }

            await _dbContext.SalesStatistics.AddRangeAsync(statistics);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
