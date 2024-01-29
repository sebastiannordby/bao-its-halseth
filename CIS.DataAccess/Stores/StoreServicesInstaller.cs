using CIS.DataAccess.Stores.Repositories;
using CIS.Library.Stores.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Stores
{
    internal static class StoreServicesInstaller
    {
        internal static IServiceCollection AddStoreServices(
            this IServiceCollection services)
        {
            return services
                .AddScoped<IStoreViewRepository, StoreViewRepository>();
        }
    }
}
