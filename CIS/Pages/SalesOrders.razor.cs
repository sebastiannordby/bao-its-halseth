using CIS.Library.Orders.Models.Import;
using CIS.Library.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using OfficeOpenXml;
using Radzen;
using Radzen.Blazor;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace CIS.Pages
{
    public partial class SalesOrders : ComponentBase
    {
        [Inject]
        public IExecuteImportService<SalesOrderImportDefinition> ImportOrderService { get; set; }

        [Inject]
        public  NotificationService NotificationService { get; set; }

        private List<SalesOrderImportDefinition> _ordersToImport;
        private IQueryable<SalesOrderImportDefinition> _ordersToImportQueryable => _ordersToImport?.AsQueryable();
        private string _importMessages;

        private int ProgressPercent { get; set; }
        private bool _importDialogHidden = true;
        private ImportState _importState;

        private RadzenDialog? _importDialog;

        private enum ImportState
        {
            Input = 0,
            Reading = 1,
            ReadingFinished = 2,
            Importing = 3
        }

        private async Task SetImportState(ImportState importState)
        {
            _importState = importState;
            await InvokeAsync(StateHasChanged);
        }

        public async Task ImportExcelFile(InputFileChangeEventArgs e)
        {
            await SetImportState(ImportState.Reading);
            try
            {
                foreach (var file in e.GetMultipleFiles(1))
                {
                    var orders = new List<SalesOrderImportDefinition>();
                    _importMessages = string.Empty;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        // copy data from file to memory stream
                        await file.OpenReadStream(20 * 500 * 1024).CopyToAsync(ms);
                        // positions the cursor at the beginning of the memory stream
                        ms.Position = 0;

                        // create ExcelPackage from memory stream
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (ExcelPackage package = new ExcelPackage(ms))
                        {
                            ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();
                            int colCount = ws.Dimension.End.Column;
                            int rowCount = ws.Dimension.End.Row;
                            int lastMessagePercentage = 0;

                            for (int row = 2; row < rowCount; row++)
                            {
                                var percentage = Convert.ToInt32(((decimal)row / rowCount) * 100);

                                if (percentage % 5 == 0 && lastMessagePercentage != percentage)
                                {
                                    NotificationService.Notify(detail: $"Leser fil {percentage}%..",duration: 2000);
                                    lastMessagePercentage = percentage;
                                }

                                try
                                {
                                    var id = ws.Cells[row, 1].Value; // ID
                                    var dated = ws.Cells[row, 2].Value; // dato
                                    var storeNumber = ws.Cells[row, 3].Value; // butikknr
                                    var productNumber = ws.Cells[row, 4].Value; // vareID
                                    var suppliersProductNumberd = ws.Cells[row, 5].Value; // varenr_lev
                                    var ean = ws.Cells[row, 6].Value; // ean
                                    var quantity = ws.Cells[row, 7].Value; // antall
                                    var quantityDelivered = ws.Cells[row, 8].Value; // antallLevert
                                    var reference = ws.Cells[row, 9].Value; // ordreref
                                    var deliveredDated = ws.Cells[row, 10].Value; // levertDato
                                    var costPriceStr = ws.Cells[row, 11].Value; // our_price
                                    var purchasePrice = ws.Cells[row, 12].Value; // innpris
                                    var shopifyOrderRefd = ws.Cells[row, 13].Value; // nettOrdreRef

                                    var idDec = (double)id;
                                    var idInt = Convert.ToInt32(idDec);

                                    var parsedQuantityDelivered = Convert.ToDecimal((double)quantityDelivered);
                                    var parsedPurchasePrice = StringDecimalToDecimal(purchasePrice as string);
                                    var parsedQuantity = Convert.ToDecimal((double)quantity);
                                    var parsedCostPrice = StringDecimalToDecimal(costPriceStr as string);
                                    var parsedProductNumber = Convert.ToInt32((double)productNumber);

                                    _importMessages += $"\r\nLeser - Ordre #{idInt} - Produkt: {productNumber}\r\n";

                                    var orderLine = new SalesOrderImportDefinition.Line()
                                    {
                                        CostPrice = parsedCostPrice,
                                        ProductNumber = parsedProductNumber,
                                        ProductName = null,
                                        EAN = ean as string,
                                        Quantity = parsedQuantity,
                                        PurchasePrice = parsedPurchasePrice,
                                        QuantityDelivered = parsedQuantityDelivered,
                                        StorePrice = null,
                                        CurrencyCode = "NOK"
                                    };

                                    var existingOrder = orders
                                        .FirstOrDefault(x => x.Number == idInt);
                                    if (existingOrder is not null)
                                    {
                                        existingOrder.Lines.Add(orderLine);
                                    }
                                    else
                                    {
                                        var parsedStoreNumber =
                                            Convert.ToInt32((double)storeNumber);

                                        var order = new SalesOrderImportDefinition()
                                        {
                                            Number = idInt,
                                            AlternateNumber = shopifyOrderRefd as string,
                                            StoreNumber = parsedStoreNumber,
                                            StoreName = "Johansen",
                                            CustomerNumber = parsedStoreNumber,
                                            CustomerName = null,
                                            DeliveredDate = DateOnly.FromDateTime(DateTime.Now),
                                            OrderDate = DateOnly.FromDateTime(DateTime.Now),
                                            Reference = reference as string,
                                            IsDeleted = false
                                        };

                                        order.Lines.Add(orderLine);

                                        orders.Add(order);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _importMessages += $"Error: {ex.Message}";
                                }
                            }

                            _ordersToImport = orders;
                        }
                    }
                }
            }
            finally
            {
                await SetImportState(ImportState.ReadingFinished);
            }
        }

        private decimal? StringDecimalToDecimal(string value)
        {
            if (value is null)
                return null;

            if (value == "NULL")
                return null;

            return Convert.ToDecimal(value);
        }

        private async Task ExecuteImport()
        {
            var result = await ImportOrderService.Import(_ordersToImport);
            if (result)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Importering velykket");
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Error, "Importering feilet");
            }
        }
    }
}
