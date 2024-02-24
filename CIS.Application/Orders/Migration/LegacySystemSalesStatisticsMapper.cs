using CIS.Application.Orders.Contracts.Import;
using CIS.Application.Orders.Migration.Contracts;
using CIS.Application.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Migration
{
    internal class LegacySystemSalesStatisticsMapper :
        IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition>
    {
        public async Task<IReadOnlyCollection<ImportSalesStatisticsDefinition>> Map(
            LegacySystemSalesStatisticsSource input, CancellationToken cancellationToken)
        {
            var importDefinitions = new List<ImportSalesStatisticsDefinition>();

            foreach(var model in input.Data)
            {
                var definition = new ImportSalesStatisticsDefinition()
                {
                    Number = (int)model.Id,
                    Date = model.Dato ?? DateTime.MinValue,
                    ProductNumber = model.VareId ?? 0,
                    CostPrice = model.OurPrice ?? 0,
                    PurchasePrice = model.Innpris ?? 0,
                    Quantity = model.Antall ?? 0,
                    StoreNumber = model.Butikknr ?? 0,
                    StorePrice = model.Utpris ?? 0,
                    CustomerNumber = model.Kundenr ?? 0,
                };

                importDefinitions.Add(definition);
            }

            return await Task.FromResult(importDefinitions);
        }
    }
}
