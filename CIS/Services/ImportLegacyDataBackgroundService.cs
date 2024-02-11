using CIS.Application.Legacy;
using CIS.Application.Orders.Models.Import;
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using CIS.Library.Customers.Models.Import;
using CIS.Library.Orders.Models.Import;
using CIS.Library.Products.Import;
using CIS.Library.Shared.Services;
using CIS.Pages;
using ExcelDataReader.Log;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CIS.Services 
{
    public class ImportLegacyDataBackgroundService
    {
        private readonly ManualResetEvent _signal = new ManualResetEvent(false);
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IHubContext<ImportLegacyDataHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        private const string ReceiveMessage = "ReceiveMessage";
        private const string Finished = "Finished";

        public ImportLegacyDataBackgroundService(
            IServiceScopeFactory scopeFactory,
            IHubContext<ImportLegacyDataHub> hubContext)
        {
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_isRunning)
                return;

            _isRunning = true;
            using var scope = _scopeFactory.CreateScope();

            var legacyDbContext = scope.ServiceProvider
                .GetRequiredService<SWNDistroContext>();

            var migrationTaskRepo = scope.ServiceProvider
                .GetRequiredService<IMigrationTaskRepo>();

            var tasks = await migrationTaskRepo.GetMigrationTasks();
            var uncompletedTasks = tasks.Where(x => !x.Executed);

            if(uncompletedTasks.Any(x => x.Type == MigrationTask.TaskType.Products))
            {
                await ImportProducts(legacyDbContext, scope);
                await migrationTaskRepo.Complete(MigrationTask.TaskType.Products);
            }

            if (uncompletedTasks.Any(x => x.Type == MigrationTask.TaskType.Customers))
            {
                await ImportCustomers(legacyDbContext, scope);
                await migrationTaskRepo.Complete(MigrationTask.TaskType.Customers);
            }

            if (uncompletedTasks.Any(x => x.Type == MigrationTask.TaskType.SalesOrders))
            {
                await ImportOrders(legacyDbContext, scope);
                await migrationTaskRepo.Complete(MigrationTask.TaskType.SalesOrders);
            }


            if (uncompletedTasks.Any(x => x.Type == MigrationTask.TaskType.SalesOrderStatistics))
            {
                await ImportSalesOrderStatistics(legacyDbContext, scope);
                await migrationTaskRepo.Complete(MigrationTask.TaskType.SalesOrderStatistics);
            }

            await _hubContext.Clients.All.SendAsync(Finished);
        }

        private async Task ImportSalesOrderStatistics(SWNDistroContext legacyDbContext, IServiceScope scope)
        {
            var importService = scope.ServiceProvider
                .GetRequiredService<IExecuteImportService<SalesStatisticsImportDefinition>>();

            await _hubContext.Clients.All.SendAsync(ReceiveMessage, "Importering av salgstall påbegynt.");

            await legacyDbContext.Salgs.ProcessEntitiesInBatches(async(sales) =>
            {
                var importDefinitions = new List<SalesStatisticsImportDefinition>();

                foreach(var sale in sales)
                {
                    var definition = new SalesStatisticsImportDefinition()
                    {
                        Number = (int) sale.Id,
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

                var success = await importService.Import(importDefinitions);
                var textLines = importDefinitions
                    .Select(x => $"Vare({x.ProductNumber})");
                var text = string.Join("\n", textLines);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({successMsg}) Salgstall:\n{text}\n";

                await _hubContext.Clients.All.SendAsync(ReceiveMessage, message);
            });

            await _hubContext.Clients.All.SendAsync(ReceiveMessage, "Importering av salgstall vellykket.");
        }

        private async Task ImportOrders(SWNDistroContext legacyDbContext, IServiceScope scope)
        {
            var importService = scope.ServiceProvider
                .GetRequiredService<IExecuteImportService<SalesOrderImportDefinition>>();

            await _hubContext.Clients.All.SendAsync(ReceiveMessage, "Importering av ordre/bestillinger påbegynt.");

            var allOrderGroupingsQuery = legacyDbContext.Ordres
                .AsNoTracking()
                .GroupBy(x => new { x.Dato, x.Butikknr })
                .Select(x => new
                {
                    Dato = x.Key.Dato,
                    Butikknr = x.Key.Butikknr
                });

            await DbSetExtensions.ProcessEntitiesInBatches(allOrderGroupingsQuery, async (orderGroupings, percentageDone) =>
            {
                var importDefinitions = new List<SalesOrderImportDefinition>();

                foreach (var test in orderGroupings)
                {
                    var grouping = await legacyDbContext.Ordres
                        .AsNoTracking()
                        .Where(x => x.Butikknr == test.Butikknr)
                        .Where(x => x.Dato == test.Dato)
                        .ToListAsync();

                    var legOrder = grouping.First();
                    var importOrder = new SalesOrderImportDefinition()
                    {
                        Number = (int)legOrder.Id,
                        AlternateNumber = legOrder.NettOrdreRef,
                        StoreNumber = legOrder.Butikknr ?? 0,
                        StoreName = null,
                        CustomerNumber = legOrder.Butikknr ?? 0,
                        CustomerName = null,
                        DeliveredDate = DateOnly.FromDateTime(DateTime.Now),
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        Reference = legOrder.Ordreref as string,
                        IsDeleted = (legOrder.Ordretype ?? "").Equals("Slettet", StringComparison.OrdinalIgnoreCase)
                    };

                    foreach (var orderLine in grouping)
                    {
                        var importLine = new SalesOrderImportDefinition.Line()
                        {
                            CostPrice = orderLine.OurPrice,
                            EAN = orderLine.Ean,
                            ProductName = null,
                            Quantity = orderLine.Antall ?? 0,
                            PurchasePrice = orderLine.Innpris,
                            QuantityDelivered = orderLine.AntallLevert ?? 0,
                            CurrencyCode = "NOK"
                        };

                        importOrder.Lines.Add(importLine);
                    }

                    importDefinitions.Add(importOrder);
                }

                var success = await importService
                    .Import(importDefinitions);

                var textLines = importDefinitions
                    .Select(x => x.Number.ToString())
                    .ToArray();

                var text = string.Join("\n", textLines);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({percentageDone}%)({successMsg}) Ordre:\n{text}\n";

                await _hubContext.Clients.All.SendAsync(ReceiveMessage, message);
            });

            await _hubContext.Clients.All.SendAsync(ReceiveMessage, "Importering av ordre/bestillinger vellykket.");
        } 

        private async Task ImportProducts(SWNDistroContext legacyDbContext, IServiceScope scope)
        {
            var importService = scope.ServiceProvider
                .GetRequiredService<IExecuteImportService<ProductImportDefinition>>();

            await _hubContext.Clients.All.SendAsync(ReceiveMessage, "Importering av varer påbegynt.");

            await legacyDbContext.Vareinfos.ProcessEntitiesInBatches(async(products) =>
            {
                var importDefinitions = new List<ProductImportDefinition>();

                foreach(var legProd in products)
                {
                    var importDef = new ProductImportDefinition()
                    {
                        Number = (int) legProd.Id,
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

                var success = await importService.Import(importDefinitions);
                var productNames = importDefinitions
                    .Select(x => x.Name)
                    .ToArray();

                var productNamesMsg = string.Join("\n", productNames);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({successMsg}) Varer:\n{productNamesMsg}\n";

                await _hubContext.Clients.All.SendAsync(ReceiveMessage, message);
            });

            await _hubContext.Clients.All.SendAsync(ReceiveMessage, "Importering av varer vellykket.");
        }

        private async Task ImportCustomers(SWNDistroContext legacyDbContext, IServiceScope scope)
        {
            var importService = scope.ServiceProvider
                .GetRequiredService<IExecuteImportService<CustomerImportDefinition>>();

            await _hubContext.Clients.All.SendAsync(ReceiveMessage, "Importering av kunder/butikker påbegynt.");

            await legacyDbContext.Butikklistes.ProcessEntitiesInBatches(async (customers) =>
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

                var success = await importService.Import(importDefinitions);
                var names = importDefinitions
                    .Select(x => x.Name)
                    .ToArray();

                var namesMsg = string.Join("\n", names);
                var successMsg = success ? "Vellykket" : "Feilet";
                var message = $"({successMsg}) Kunder:\n{namesMsg}\n";

                await _hubContext.Clients.All.SendAsync(ReceiveMessage, message);
            });
        }
    }

    public static class DbSetExtensions
    {
        public static async Task ProcessEntitiesInBatches<T>(
            this DbSet<T> dbSet,
            Func<IEnumerable<T>, Task> processBatch)
            where T : class
        {
            var totalRecords = await dbSet.CountAsync();
            var batchSize = totalRecords > 2000 ? 500 : 50;

            var currentPercentage = 0;
            var offset = 0;

            while (true)
            {
                // Query for the next batch of entities
                IQueryable<T> query = dbSet;
                var batch = await query
                    .Skip(offset)
                    .Take(batchSize)
                    .ToListAsync();

                var numberOfRecordsProcessed = offset + batch.Count;
                var newPercentage = (int)Math.Round(
                    (double)numberOfRecordsProcessed / totalRecords * 100);

                if (newPercentage >= currentPercentage + 5)
                {
                    currentPercentage = newPercentage;
                }

                if (batch.Count == 0)
                    break; // No more entities to process

                // Process the batch
                await processBatch(batch);

                offset += batch.Count;
            }
        }

        public static async Task ProcessEntitiesInBatches<T>(
            this IQueryable<T> dbSet,
            Func<IEnumerable<T>, int, Task> processBatch)
            where T : class
        {
            var totalRecords = await dbSet.CountAsync();
            var batchSize = 100;
            var currentPercentage = 0;
            var offset = 0;

            while (true)
            {
                // Query for the next batch of entities
                IQueryable<T> query = dbSet;
                var batch = await query
                    .Skip(offset)
                    .Take(batchSize)
                    .ToListAsync();

                var batchCount = batch.Count;

                var numberOfRecordsProcessed = offset + batchCount;
                var newPercentage = (int)Math.Round(
                    (double)numberOfRecordsProcessed / totalRecords * 100);

                if (batchCount == 0)
                    break; // No more entities to process

                // Process the batch
                await processBatch(batch, newPercentage);

                offset += batchCount;
            }
        }
    }
}