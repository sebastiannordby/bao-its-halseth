﻿using CIS.WebApp.Extensions;
using CIS.Library.Customers.Models;
using CIS.Library.Customers.Models.Import;
using CIS.Library.Customers.Repositories;
using CIS.Library.Shared.Services;
using CIS.Library.Stores.Models;
using CIS.WebApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Radzen.Blazor;

namespace CIS.WebApp.Components.Pages
{
    public partial class Customers : ComponentBase
    {
        [Inject] 
        public NotificationService NotificationService { get; set; }

        [Inject]
        public ICustomerViewRepository CustomerViewRepository { get; set; }

        [Inject] 
        public ImportService ImportService { get; set; }

        [Inject]
        public IExecuteImportService<CustomerImportDefinition> ImportCustomerService { get; set; }

        public const int OVERVIEW_TAB_INDEX = 0;
        public const int IMPORT_TAB_INDEX = 1;

        private int _selectedTabIndex = OVERVIEW_TAB_INDEX;

        private string _importMessages;
        private List<CustomerView> _preViewCustomers = new();
        private ImportState _importState = ImportState.Input;
        private bool _preViewHidden = true;

        private List<CustomerImportDefinition> _customerImportDefinitions;
        private RadzenDataGrid<CustomerImportDefinition> _importDataGrid;

        private IReadOnlyCollection<CustomerView> _customers;
        private RadzenDataGrid<CustomerView> _overviewGrid;

        protected override async Task OnInitializedAsync()
        {
            ImportService.ImportStateChanged = (state) =>
            {
                _importState = state;
            };

            await LoadOverviewData();
        }

        public async Task LoadOverviewData()
        {
            _customers = await CustomerViewRepository.List();

            if(_overviewGrid is not null)
            {
                await _overviewGrid.RefreshDataAsync();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task ClearImportData()
        {
            _customerImportDefinitions = new();

            if (_importDataGrid is not null)
            {
                await _importDataGrid.RefreshDataAsync();
            }
        }

        private async Task ExecuteImport()
        {
            var res = await ImportCustomerService.Import(_customerImportDefinitions);

            if(res)
            {
                NotificationService.Notify(
                    NotificationSeverity.Success, "Importering vellykket");
                _selectedTabIndex = OVERVIEW_TAB_INDEX;
                await LoadOverviewData();
                await ClearImportData();
            }
        }

        private async Task ImportExcelFile(InputFileChangeEventArgs e)
        {
            _customerImportDefinitions = new();

            var file = e.GetMultipleFiles(1).FirstOrDefault();

            await ImportService.StartImportAsync(file, (cellData) =>
            {
                var (rowIndex, workSheet) = cellData;

                try
                {
                    var storeNumber = workSheet.Cells[rowIndex, 1].Value.ToInt32();
                    var storeName = workSheet.Cells[rowIndex, 2].Value as string;
                    var addressLine = workSheet.Cells[rowIndex, 3].Value as string;
                    var postalCode = workSheet.Cells[rowIndex, 4].Value.ToInt32();
                    var postalOffice = workSheet.Cells[rowIndex, 5].Value as string;
                    var emailAddress = workSheet.Cells[rowIndex, 6].Value as string;
                    var regionNumber = workSheet.Cells[rowIndex, 7].Value.ToInt32();
                    var regionName = workSheet.Cells[rowIndex, 8].Value as string;
                    var location = workSheet.Cells[rowIndex, 9].Value as string;
                    var active = workSheet.Cells[rowIndex, 10].Value.ToInt32();
                    var creditBlock = workSheet.Cells[rowIndex, 11].Value.ToInt32();
                    var customerNumber = workSheet.Cells[rowIndex, 12].Value.ToInt32();
                    var mobilePhone = workSheet.Cells[rowIndex, 13].Value.ToString();
                    if (!customerNumber.HasValue)
                        return;

                    var importDef = new CustomerImportDefinition()
                    {
                        Number = customerNumber.Value,
                        Name = storeName,
                        ContactPersonName = storeName,
                        ContactPersonEmailAddress = emailAddress,
                        ContactPersonPhoneNumber = mobilePhone,
                        IsActive = active == 0 ? false : true,
                        CustomerGroupNumber = null,
                        Store = new CustomerImportDefinition.StoreDefinition()
                        {
                            Name = storeName,
                            Number = storeNumber.Value,
                            AddressLine = addressLine,
                            AddressPostalCode = postalCode.ToString(),
                            AddressPostalOffice = postalOffice,
                            RegionNumber = regionNumber,
                            RegionName = regionName,
                        }
                    };

                    _customerImportDefinitions.Add(importDef);
                }
                catch(Exception)
                {

                }
            });

            await InvokeAsync(StateHasChanged);
            await _importDataGrid.RefreshDataAsync();
        }
    }
}