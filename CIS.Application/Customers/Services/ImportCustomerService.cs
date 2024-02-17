using CIS.Application.Customers.Models;
using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Shared.Extensions;
using CIS.Application.Shared.Services;
using CIS.Application.Stores.Models;
using CIS.Library.Customers.Models.Import;
using CIS.Library.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CIS.Application.Customers.Services
{
    internal class ImportCustomerService : 
        IExecuteImportService<CustomerImportDefinition>,
        IMigrateLegacyService<Butikkliste>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _swnDistroContext;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hub;

        public ImportCustomerService(
            CISDbContext dbContext,
            SWNDistroContext swnDistroContext,
            IHubContext<ImportLegacyDataHub, IListenImportClient> hub)
        {
            _dbContext = dbContext;
            _swnDistroContext = swnDistroContext;
            _hub = hub;
        }

        public async Task<bool> Import(IEnumerable<CustomerImportDefinition> importDefinitions)
        {
            var lookedUpRegions = new List<RegionImportStruct>();
            var customers = new List<CustomerDao>();
            var stores = new List<StoreDao>();
            var regions = new List<RegionDao>();

            foreach (var customerDefinition in importDefinitions)
            {
                var doesExist = await _dbContext.Customers
                    .AnyAsync(x => x.Number == customerDefinition.Number);
                if (doesExist)
                    continue;

                var customerDao = new CustomerDao()
                {
                    Id = Guid.NewGuid(),
                    Number = customerDefinition.Number,
                    Name = customerDefinition.Name,
                    ContactPersonEmailAddress = customerDefinition.ContactPersonEmailAddress,
                    ContactPersonName = customerDefinition.ContactPersonName,
                    ContactPersonPhoneNumber = customerDefinition.ContactPersonPhoneNumber,
                    CustomerGroupId = null,
                    IsActive = customerDefinition.IsActive
                };

                customers.Add(customerDao);

                if (customerDefinition.Store is not null)
                {
                    var storeDefinition = customerDefinition.Store;
                    var regionId = null as Guid?;

                    if(storeDefinition.RegionNumber.HasValue)
                    {
                        var region = regions
                            .FirstOrDefault(x => x.Number == storeDefinition.RegionNumber);
                        if(region is null)
                        {
                            region = new RegionDao()
                            {
                                Id = Guid.NewGuid(),
                                Number = storeDefinition.RegionNumber.Value,
                                Name = storeDefinition.Name ?? "IKKE DEFINERT",
                            };

                            regions.Add(region);
                        }

                        regionId = region.Id;
                    }

                    var storeDao = new StoreDao()
                    {
                        Id = Guid.NewGuid(),
                        Number = storeDefinition.Number,
                        Name = customerDefinition.Name,
                        AddressLine = storeDefinition.AddressLine,
                        AddressPostalCode = storeDefinition.AddressPostalCode,
                        AddressPostalOffice = storeDefinition.AddressPostalOffice,
                        IsActive = true,
                        RegionId = regionId,
                        OwnerCustomerId = customerDao.Id
                    };

                    stores.Add(storeDao);
                }
            }

            await _dbContext.Customers.AddRangeAsync(customers);
            await _dbContext.Stores.AddRangeAsync(stores);
            await _dbContext.Regions.AddRangeAsync(regions);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task Migrate()
        {
            await _hub.Clients.All.ReceiveMessage("Importering av kunder/butikker påbegynt.");

            await _swnDistroContext.Butikklistes.ProcessEntitiesInBatches(async (customers, percentage) =>
            {
                var importDefinitions = new List<CustomerImportDefinition>();

                foreach (var leg in customers)
                {
                    var importDef = new CustomerImportDefinition()
                    {
                        Number = leg.Kundenr ?? leg.Butikknr,
                        Name = leg.Butikknavn,
                        ContactPersonName = leg.Butikknavn,
                        ContactPersonEmailAddress = leg.Epost,
                        ContactPersonPhoneNumber = leg.Telefon?.ToString(),
                        IsActive = leg.Aktiv ?? true,
                        CustomerGroupNumber = null,
                        Store = new CustomerImportDefinition.StoreDefinition()
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

                var success = await Import(importDefinitions);
                var names = importDefinitions
                    .Select(x => x.Name)
                    .ToArray();

                var namesMsg = string.Join("\n", names);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({percentage}%)({successMsg}) Kunder:\n{namesMsg}\n";

                await _hub.Clients.All.ReceiveMessage(message);
            });
        }

        private struct RegionImportStruct
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }
    }
}
