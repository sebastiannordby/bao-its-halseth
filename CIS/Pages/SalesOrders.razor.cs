﻿using CIS.Library.Orders.Models.Import;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.FluentUI.AspNetCore.Components;
using OfficeOpenXml;
using System.ComponentModel;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace CIS.Pages
{
    public partial class SalesOrders : ComponentBase
    {
        private List<SalesOrderImportDefinition> _ordersToImport;
        private IQueryable<SalesOrderImportDefinition> _ordersToImportQueryable => _ordersToImport?.AsQueryable();
        private int ProgressPercent { get; set; }
        private bool _importDialogHidden = true;
        private List<string> _importErrorMessages;
        private ImportState _importState;

        private FluentDialog? _importDialog;

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
                    _importErrorMessages = new List<string>();

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

                            for (int row = 2; row < rowCount; row++)
                            {
                                var per = ((decimal) row / rowCount);
                                ProgressPercent = Convert.ToInt32(per * 100);

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
                                    _importErrorMessages.Add(ex.Message);
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
                await _importDialog.CloseAsync();
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
    }
}