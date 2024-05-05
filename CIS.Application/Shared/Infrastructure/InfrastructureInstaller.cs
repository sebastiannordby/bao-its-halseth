using CIS.Application.Features;
using CIS.Application.Features.Orders.Infrastructure;
using CIS.Application.Features.Products;
using CIS.Application.Features.Stores.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CIS.Application.Shared.Infrastructure
{
    internal static class InfrastructureInstaller
    {
        internal static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            return services
                .AddTransient<IStoreQueries, CustomerQueries>()
                .AddTransient<IProductQueries, ProductQueries>()
                .AddTransient<ISalesQueries, SalesQueries>()
                .AddTransient<INotifyClientService, SignalRNotifyClientService>();
        } 
    }
}
