using CIS.Library.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using OfficeOpenXml;
using Radzen;
using Radzen.Blazor;
using LicenseContext = OfficeOpenXml.LicenseContext;
using CIS.WebApp.Extensions;
using CIS.WebApp.Services;
using OfficeOpenXml.Style;
using CIS.Library.Orders.Models;
using CIS.Application.Legacy;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq.Dynamic.Core;
using CIS.Application.Orders.Contracts;
using CIS.Application.Orders.Infrastructure;
using CIS.Application.Orders.Import.Contracts;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class SalesOrders : ComponentBase, IDisposable
    {
        [Inject]
        public required IProcessImportCommandService<ImportSalesOrderCommand> ImportOrderService { get; set; }

        [Inject]
        public required NotificationService NotificationService { get; set; }

        [Inject]
        public required ISalesQueries SalesOrderViewRepository { get; set; }

        [Inject]
        public required ImportService ImportService { get; set; }

        private List<ImportSalesOrderDefinition>? _orderImportDefinitions;
        private RadzenDataGrid<ImportSalesOrderDefinition> _importDataGrid;

        private int _salesOrderCount;
        private IReadOnlyCollection<SalesOrderView>? _salesOrders;
        private RadzenDataGrid<SalesOrderView>? _overviewGrid;

        private string? _importMessages;

        private int ProgressPercent { get; set; }
        private bool _importDialogHidden = true;

        private RadzenDialog? _importDialog;

        public const int OVERVIEW_TAB_INDEX = 0;
        public const int IMPORT_TAB_INDEX = 1;

        private int _selectedTabIndex = OVERVIEW_TAB_INDEX;

        private CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadOverviewData();
        }

        private async Task LoadSalesOrders(LoadDataArgs args)
        {
            var query = SalesOrderViewRepository.Query();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _salesOrderCount = query.Count();
            _salesOrders = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync();
        }

        private async Task LoadOverviewData()
        {
            _salesOrders = await SalesOrderViewRepository.List(1000, 0);

            if(_overviewGrid is not null)
            {
                await _overviewGrid.RefreshDataAsync();
            }
        }

        private async Task ClearImportData()
        {
            _orderImportDefinitions = new();

            if (_importDataGrid is not null)
            {
                await _importDataGrid.RefreshDataAsync();
            }
        }

        public async Task ImportExcelFile(InputFileChangeEventArgs e)
        {
            _orderImportDefinitions = new();

            await ImportService.UploadFileAsync(e.GetMultipleFiles().First());

            await ImportService.StartImportAsync(
                e.GetMultipleFiles().First(), (cellData) =>
            {
                var (row, ws) = cellData;
                var orderNumber = ws.Cells[row, 1].Value.ToInt32(); // ID
                var orderDate = ws.Cells[row, 2].Value?.ToString(); // dato
                var storeNumber = ws.Cells[row, 3].Value; // butikknr
                var productNumber = ws.Cells[row, 4].Value.ToInt32(); // vareID
                var suppliersProductNumber = ws.Cells[row, 5].Value; // varenr_lev
                var ean = ws.Cells[row, 6].Value.AsExcelString(); // ean
                var quantity = ws.Cells[row, 7].Value.ToDecimal(); // antall
                var quantityDelivered = ws.Cells[row, 8].Value.ToDecimal(); // antallLevert
                var reference = ws.Cells[row, 9].Value; // ordreref
                var isSentToExternal = ws.Cells[row, 10].Value; // ordreref
                var transferedDateExternal = ws.Cells[row, 11].Value; // ordreref
                var type = ws.Cells[row, 12].Value.AsExcelString(); // ordreref
                var deliveredDated = ws.Cells[row, 13].Value; // levertDato
                var costPrice = ws.Cells[row, 14].Value.ToDecimal(); // our_price
                var purchasePrice = ws.Cells[row, 14].Value.ToDecimal(); // innpris
                var shopifyOrderRefd = ws.Cells[row, 16].Value.AsExcelString(); // nettOrdreRef
                if (!productNumber.HasValue)
                    return;

                var isDeleted = (type ?? "") == "slettet";

                _importMessages += $"\r\nLeser - Ordre #{orderNumber} - Produkt: {productNumber}\r\n";

                var orderLine = new ImportSalesOrderDefinition.Line()
                {
                    CostPrice = costPrice,
                    ProductNumber = productNumber.Value,
                    ProductName = null,
                    EAN = ean as string,
                    Quantity = quantity ?? 0,
                    PurchasePrice = purchasePrice,
                    QuantityDelivered = quantityDelivered ?? 0,
                    StorePrice = null,
                    CurrencyCode = "NOK"
                };

                var existingOrder = _orderImportDefinitions
                    .FirstOrDefault(x => x.Number == orderNumber);
                if (existingOrder is not null)
                {
                    existingOrder.Lines.Add(orderLine);
                }
                else
                {
                    var parsedStoreNumber =
                        Convert.ToInt32((double)storeNumber);

                    var order = new ImportSalesOrderDefinition()
                    {
                        Number = orderNumber.Value,
                        AlternateNumber = shopifyOrderRefd,
                        StoreNumber = parsedStoreNumber,
                        StoreName = null,
                        CustomerNumber = parsedStoreNumber,
                        CustomerName = null,
                        DeliveredDate = DateOnly.FromDateTime(DateTime.Now),
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        Reference = reference as string,
                        IsDeleted = isDeleted
                    };

                    order.Lines.Add(orderLine);

                    _orderImportDefinitions.Add(order);
                }
            });

            await _importDataGrid.RefreshDataAsync();
        }

        private async Task ExecuteImport()
        {
            if (_orderImportDefinitions is null)
                return;

            var command = new ImportSalesOrderCommand()
            {
                Definitions = _orderImportDefinitions
            };
            
            var result = await ImportOrderService
                .Import(command, _cts.Token);
            if (result)
            {
                NotificationService.Notify(
                    NotificationSeverity.Success, "Importering vellykket");
                _selectedTabIndex = OVERVIEW_TAB_INDEX;
                await LoadOverviewData();
                await ClearImportData();
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Error, "Importering feilet");
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
