using CIS.Application;
using CIS.Application.Features;
using CIS.Application.Legacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CIS.Tests.Domain
{
    public class DomainTestFixture : IDisposable
    {
        public ServiceCollection GetServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<CISDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(DomainTestFixture), b =>
                {
                    b.EnableNullChecks(false);
                });
            });

            serviceCollection.AddDbContext<SWNDistroContext>(options =>
            {
                options.UseInMemoryDatabase($"Legacy_{nameof(DomainTestFixture)}", b =>
                {
                    b.EnableNullChecks(false);
                });
            });

            serviceCollection.AddLogging();

            return serviceCollection;
        }

        public INotifyClientService AddNotifyClientServiceMock(ServiceCollection services)
        {
            var notifyClientServiceMock = Substitute.For<INotifyClientService>();
            services.AddSingleton(notifyClientServiceMock);

            return notifyClientServiceMock;
        }

        public void Dispose()
        {

        }
    }
}
