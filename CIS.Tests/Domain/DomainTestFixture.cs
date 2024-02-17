using CIS.Application;
using CIS.Application.Legacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Tests.Domain
{
    public class DomainTestFixture : IDisposable
    {
        public ServiceCollection GetServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<CISDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(DomainTestFixture), b => {
                    b.EnableNullChecks(false);
                });
            });

            serviceCollection.AddDbContext<SWNDistroContext>(options =>
            {
                options.UseInMemoryDatabase($"Legacy_{nameof(DomainTestFixture)}", b => {
                    b.EnableNullChecks(false);
                });
            });

            return serviceCollection;
        }

        public void Dispose()
        {

        }
    }
}
