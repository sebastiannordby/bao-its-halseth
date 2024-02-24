using CIS.Application.Orders.Infrastructure;
using CIS.Application.Products;
using CIS.Application.Stores.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .AddTransient<ISalesQueries, SalesQueries>();
        } 
    }
}
