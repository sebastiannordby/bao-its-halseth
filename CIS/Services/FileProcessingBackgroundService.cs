using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CIS.Services 
{
    public class FileProcessingBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public FileProcessingBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Polling or dequeueing mechanism to retrieve files for processing
                    // Example: Check a database table or a queue for files to process

                    // For demonstration purposes, we'll simulate processing by sleeping for a few seconds
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                    // Process the file
                    await ProcessNextFileAsync();
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur during processing
                    Console.WriteLine($"An error occurred while processing the file: {ex.Message}");
                }
            }
        }

        private async Task ProcessNextFileAsync()
        {
            // Perform file processing logic here
            // Example: Read the file, parse its contents, and store data in the database

            // For demonstration purposes, we'll simply log a message
            Console.WriteLine("Processing next file...");
        }
    }
}