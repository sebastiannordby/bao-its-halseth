using CIS.Application;
using CIS.Application.Shopify;
using System.Globalization;

namespace CIS.Integration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("ConnectionStrings:DefaultConnection must be configured in user secrets or appsettings.json.");

            var legacyConnectionString = builder.Configuration
                .GetConnectionString("LegacyConnection");
            if (string.IsNullOrWhiteSpace(legacyConnectionString))
                throw new ArgumentException("ConnectionStrings:LegacyConnection must be configured in user secrets or appsettings.json.");

            builder.Services.AddHostedService<ShopifyWorker>();
            builder.Services.AddCISShopifySharp(builder.Configuration);
            builder.Services.AddSWNDistroLegacyDatabase(legacyConnectionString);
            builder.Services.AddCISDatabase(connectionString);
            builder.Services.AddCISLogging();

            var host = builder.Build();
            host.Run();
        }
    }
}