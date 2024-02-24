using CIS.Application;
using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
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

        public IHubContext<ImportLegacyDataHub, IListenImportClient> AddLegacyHubMock(ServiceCollection services)
        {
            var hubMock = Substitute.For<IHubContext<ImportLegacyDataHub, IListenImportClient>>();
            services.AddSingleton(hubMock);

            return hubMock;
        }

        public void Dispose()
        {

        }
    }
}
