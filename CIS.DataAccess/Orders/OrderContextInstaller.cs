using CIS.DataAccess.Orders.Services;
using CIS.Library.Orders.Models.Import;
using CIS.Library.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Orders
{
    internal static class OrderContextInstaller
    {
        internal static IServiceCollection AddOrderServices(
            this IServiceCollection services)
        {
            return services
                .AddScoped<IExecuteImportService<SalesOrderImportDefinition>, ImportOrderService>();
        }
    }
}
