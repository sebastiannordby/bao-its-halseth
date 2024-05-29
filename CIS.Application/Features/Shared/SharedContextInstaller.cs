using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Shared
{
    internal static class SharedContextInstaller
    {
        internal static IServiceCollection AddSharedFeature(
            this IServiceCollection services)
        {
            return services
                .AddTransient<DemoService>();
        }
    }
}
