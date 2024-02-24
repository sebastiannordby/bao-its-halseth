using CIS.Application.Shared.Services;
using CIS.Application.Stores.Migration.Contracts;
using CIS.Application.Stores.Models.Import;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Stores.Migration
{
    internal sealed class LegacySystemCustomerMapper :
        IMigrationMapper<LegacySystemCustomerSource, ImportCustomerDefinition>
    {
        public async Task<IReadOnlyCollection<ImportCustomerDefinition>> Map(LegacySystemCustomerSource input, CancellationToken cancellationToken)
        {
            var importDefinitions = new List<ImportCustomerDefinition>();

            foreach (var leg in input.Data)
            {
                var importDef = new ImportCustomerDefinition()
                {
                    Number = leg.Kundenr ?? leg.Butikknr,
                    Name = leg.Butikknavn,
                    ContactPersonName = leg.Butikknavn,
                    ContactPersonEmailAddress = leg.Epost,
                    ContactPersonPhoneNumber = leg.Telefon?.ToString(),
                    IsActive = leg.Aktiv ?? true,
                    CustomerGroupNumber = null,
                    Store = new ImportCustomerDefinition.StoreDefinition()
                    {
                        Name = leg.Butikknavn,
                        Number = leg.Butikknr,
                        AddressLine = leg.Gateadresse,
                        AddressPostalCode = leg.Postnr?.ToString(),
                        AddressPostalOffice = leg.Poststed,
                        RegionName = leg.RegionNavn,
                        RegionNumber = leg.RegionNr,
                    }
                };

                importDefinitions.Add(importDef);
            }

            return await Task.FromResult(importDefinitions);
        }
    }
}
