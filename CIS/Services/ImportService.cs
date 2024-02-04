using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Identity.Client;
using OfficeOpenXml;
using Radzen;
using System.Reflection.Metadata.Ecma335;

namespace CIS.Services
{
    public class ImportService(NotificationService notificationService)
    {
        private readonly NotificationService _notificationService = notificationService;

        public Action<ImportState> ImportStateChanged { get; set; }

        public Action<decimal> PercantageChanged { get; set; }

        public Action<string> ImportMessageReceived { get; set; }

        public Action<(int columnIndex, object cellValue)> CellRead { get; set; }

        public async Task StartImportAsync(
            IBrowserFile file, 
            Action<(int currentRow , ExcelWorksheet workSheet)> columnIndexMapping) 
        {
            ImportStateChanged?.Invoke(ImportState.Reading);

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var mem = new MemoryStream();
                await file.OpenReadStream(20 * 500 * 1024).CopyToAsync(mem);

                using var package = new ExcelPackage(mem);
                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if(ws == null)
                {
                    _notificationService.Notify(NotificationSeverity.Error, detail: "Kunne ikke lese fil\navslutter...");
                    return;
                }

                int colCount = ws.Dimension.End.Column;
                int rowCount = ws.Dimension.End.Row;
                int startRow = 2; // Array starts at 1 and we want to skip the header in the excel file
                int lastMessagePercentage = 0;

                for (int row = startRow; row < rowCount; row++)
                {
                    var percentage = Convert.ToInt32(((decimal)row / rowCount) * 100);
                    if (percentage % 5 == 0 && lastMessagePercentage != percentage)
                    {
                        _notificationService.Notify(detail: $"Leser fil {percentage}%..", duration: 2000);
                        lastMessagePercentage = percentage;
                    }

                    columnIndexMapping.Invoke((row, ws));
                }

            }
            catch (Exception ex)
            {
                _notificationService.Notify(NotificationSeverity.Error, detail: ex.Message);
            }
        }
    }

    public enum ImportState
    {
        Input = 0,
        Reading = 1,
        ReadingFinished = 2,
        Importing = 3
    }
}
