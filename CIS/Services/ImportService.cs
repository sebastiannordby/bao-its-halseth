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
             Action<(int currentRow, ExcelWorksheet workSheet)> columnIndexMapping)
        {
            ImportStateChanged?.Invoke(ImportState.Reading);

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var stream = file.OpenReadStream(20 * 500 * 1024);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                using var package = new ExcelPackage(ms);

                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null)
                {
                    _notificationService.Notify(
                        NotificationSeverity.Error, detail: "Kunne ikke lese fil\navslutter...");
                    return;
                }

                int colCount = ws.Dimension.End.Column;
                int rowCount = ws.Dimension.End.Row;
                int startRow = 2; // Array starts at 1 and we want to skip the header in the excel file
                int lastMessagePercentage = 0;

                // Define batch size
                int batchSize = 1000;
                for (int row = startRow; row < rowCount; row += batchSize)
                {
                    var percentage = Convert.ToInt32(((decimal)row / rowCount) * 100);
                    if (percentage % 10 == 0 && lastMessagePercentage != percentage)
                    {
                        _notificationService.Notify(detail: $"Leser fil {percentage}%..", duration: 2000);
                        lastMessagePercentage = percentage;
                    }

                    // Process rows in batches
                    var count = Math.Min(row + batchSize, rowCount);
                    for (int batchRow = row; batchRow < count; batchRow++)
                    {
                        columnIndexMapping.Invoke((batchRow, ws));
                    }

                    // Yield control to keep the UI responsive
                    await Task.Yield();
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(NotificationSeverity.Error, detail: ex.Message);
            }
        }

        public async Task UploadFileAsync(IBrowserFile file)
        {
            try
            {
                var filePath = Path.Combine("wwwroot", "uploads", file.Name);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.OpenReadStream().CopyToAsync(stream);
                }

                // BackgroundTaskService.Enqueue(() => ProcessFileAsync(filePath));
                _notificationService.Notify(NotificationSeverity.Success, detail: "Hei");
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
