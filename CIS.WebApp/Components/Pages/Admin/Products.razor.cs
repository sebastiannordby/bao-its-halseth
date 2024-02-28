using CIS.WebApp.Components;
using CIS.WebApp.Extensions;
using CIS.Library.Shared.Services;
using CIS.WebApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Radzen.Blazor;
using CIS.WebApp.Components.Dialogs;
using CIS.Application.Features.Products;
using CIS.Application.Features.Products.Models;
using CIS.Application.Features.Products.Import.Contracts;
using CIS.Application.Features.Products.Models.Import;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class Products : ComponentBase
    {
        [Inject]
        public required IProcessImportCommandService<ImportProductCommand> ProductImportService { get; set; }

        [Inject]
        public required ImportService ImportService { get; set; }

        [Inject]
        public required NotificationService NotificationService { get; set; }

        [Inject]
        public required DialogService DialogService { get; set; }

        [Inject]
        public required IProductQueries ProductQueries { get; set; }

        public const int OVERVIEW_TAB_INDEX = 0;
        public const int IMPORT_TAB_INDEX = 1;

        private int _selectedTabIndex = OVERVIEW_TAB_INDEX;

        private IReadOnlyCollection<ProductView> _products;
        private RadzenDataGrid<ProductView> _overviewGrid;

        private List<ImportProductDefinition> _productImportDefinitions;
        private RadzenDataGrid<ImportProductDefinition> _importDataGrid;

        private bool _showDetailDialog;
        private ProductView _detailDialogProduct;

        protected override async Task OnInitializedAsync()
        {
            await LoadOverviewData();
        }

        private async Task LoadOverviewData()
        {
            _products = await ProductQueries.List();

            if(_overviewGrid is not null)
            {
                await _overviewGrid.RefreshDataAsync();
            }
        }
        
        private async Task OnRowDoubleClick(DataGridRowMouseEventArgs<ProductView> args)
        {
            await ShowDetailDialog(args.Data);
        }

        private async Task ShowDetailDialog(ProductView product)
        {
            _detailDialogProduct = product;
            _showDetailDialog = true;

            var dialogOptions =
              new DialogOptions()
              {
                  Width = "700px",
                  Height = "512px",
                  Resizable = true,
                  Draggable = true,
                  CloseDialogOnOverlayClick = true
              };

            var dialogParameters = new Dictionary<string, object>() 
            {
                { nameof(ProductDetailDialog.Product), product }
            };

            await DialogService.OpenAsync<ProductDetailDialog>(
                $"Vare {_detailDialogProduct.Name}",
              dialogParameters, 
              dialogOptions);
        }

        private async Task ExecuteImport()
        {
            var result = await ProductImportService.Import(new() 
            { 
                Definitions = _productImportDefinitions 
            }, CancellationToken.None);

            if(result)
            {
                NotificationService.Notify(
                    NotificationSeverity.Success, "Importering vellykket");
                _selectedTabIndex = OVERVIEW_TAB_INDEX;
                await LoadOverviewData();
                await ClearImportData();
            }
        }

        private async Task ClearImportData()
        {
            _productImportDefinitions = new();

            if(_importDataGrid is not null)
            {
                await _importDataGrid.RefreshDataAsync();
            }
        }

        private async Task ImportExcelFile(InputFileChangeEventArgs e)
        {
            _productImportDefinitions = new();

            var file = e.GetMultipleFiles(1).FirstOrDefault();

            await ImportService.StartImportAsync(file, (cellData) =>
            {
                var (rowIndex, workSheet) = cellData;

                try
                {
                    var number = workSheet.Cells[rowIndex, 1].Value.ToInt32();
                    var name = workSheet.Cells[rowIndex, 2].Value as string;
                    var alternateName = workSheet.Cells[rowIndex, 3].Value as string;
                    var supplierProductNumber = workSheet.Cells[rowIndex, 4].Value?.ToString();
                    var ean = workSheet.Cells[rowIndex, 5].Value?.ToString();
                    var isActive = workSheet.Cells[rowIndex, 6].Value.ToInt32();
                    var groupNumber = workSheet.Cells[rowIndex, 7].Value.ToInt32();
                    var groupName = workSheet.Cells[rowIndex, 8].Value as string;
                    var costPrice = workSheet.Cells[rowIndex, 9].Value.ToDecimal();
                    var purchasePrice = workSheet.Cells[rowIndex, 10].Value.ToDecimal();
                    var storePrice = workSheet.Cells[rowIndex, 11].Value.ToDecimal();
                    var currencyCode = "NOK";
                    if (!number.HasValue)
                        return;

                    var importDef = new ImportProductDefinition()
                    {
                        Number = number.Value,
                        Name = name,
                        AlternateName = alternateName,
                        SuppliersProductNumber = supplierProductNumber,
                        EAN = ean,
                        IsActive = isActive == 0 ? false : true,
                        ProductGroupNumber = groupNumber,
                        ProductGroupName = groupName,
                        CostPrice = costPrice,
                        PurchasePrice = purchasePrice,
                        StorePrice = storePrice,
                        CurrencyCode = currencyCode,
                    };

                    _productImportDefinitions.Add(importDef);
                }
                catch (Exception ex)
                {

                }
            });

            await InvokeAsync(StateHasChanged);
            await _importDataGrid.RefreshDataAsync();
        }
    }
}
