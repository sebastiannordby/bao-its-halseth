using CIS.Application.Products.Migration.Contracts;
using CIS.Application.Shared.Services;
using CIS.Library.Products.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Products.Migration
{
    internal class LegacySystemProductMapper :
        IMigrationMapper<LegacySystemProductSource, ImportProductDefinition>
    {
        public async Task<IReadOnlyCollection<ImportProductDefinition>> Map(
            LegacySystemProductSource input, CancellationToken cancellationToken)
        {
            var importDefinitions = new List<ImportProductDefinition>();

            foreach (var legProd in input.Data)
            {
                var importDef = new ImportProductDefinition()
                {
                    Number = (int)legProd.Id,
                    AlternateNumber = legProd.VarenrSwn,
                    Name = legProd.Varebeskrivelse2,
                    AlternateName = legProd.VaretekstAlternativ,
                    SuppliersProductNumber = legProd.VarenrLev,
                    EAN = legProd.Ean,
                    IsActive = legProd.Aktiv ?? false,
                    CurrencyCode = "NOK",
                    CostPrice = legProd.OurPrice,
                    PurchasePrice = legProd.Innpris,
                    StorePrice = legProd.Utpris
                };

                importDefinitions.Add(importDef);
            }

            return await Task.FromResult(importDefinitions);
        }
    }
}
