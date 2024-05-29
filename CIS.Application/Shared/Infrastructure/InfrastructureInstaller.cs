using CIS.Application.Features;
using CIS.Application.Features.Orders.Infrastructure;
using CIS.Application.Features.Orders.Migration.Infrastructure;
using CIS.Application.Features.Products;
using CIS.Application.Features.Products.Infrastructure;
using CIS.Application.Features.Stores.Infrastructure;
using CIS.Application.Shared.Infrastructure.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace CIS.Application.Shared.Infrastructure
{
    internal static class InfrastructureInstaller
    {
        internal static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            return services
                .AddCommands()
                .AddQueries();
        }

        internal static IServiceCollection AddQueries(
            this IServiceCollection services)
        {
            return services
                .AddTransient<IStoreQueries, CustomerQueries>()
                .AddTransient<IProductQueries, ProductQueries>()
                .AddTransient<ISalesQueries, SalesQueries>()
                .AddTransient<INotifyClientService, SignalRNotifyClientService>();
        } 

        internal static IServiceCollection AddCommands(
            this IServiceCollection services)
        {
            return services
                .AddTransient<ISalesCommands, SalesCommands>()
                .AddTransient<IMigrationCommands, MigrationCommands>()
                .AddTransient<IProductCommands, ProductCommands>()
                .AddTransient<IStoreCommands, StoreCommands>();
        }
    }
}
