using CIS.Application.Features;
using CIS.Application.Features.Orders;
using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Migration.Contracts;
using CIS.Application.Legacy;
using CIS.Application.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CIS.Tests.Domain.Orders.Migration
{
    public class MigrateLegacySalgServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrateLegacyService<Salg> _sut;
        private readonly INotifyClientService _notifyClientService;
        private readonly IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition> _mapperMock;

        public MigrateLegacySalgServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();

            serviceCollection.AddOrderFeature();

            _notifyClientService = fixture.AddNotifyClientServiceMock(serviceCollection);

            _mapperMock = Substitute.For<IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition>>();
            serviceCollection.AddSingleton(_mapperMock);

            var services = serviceCollection.BuildServiceProvider();

            _sut = services.GetRequiredService<IMigrateLegacyService<Salg>>();
        }

        [Fact]
        public async Task ShouldSendMessages()
        {
            await _sut.Migrate(CancellationToken.None);
            await _notifyClientService
                .Received()
                .SendPlainText(Arg.Any<string>());
        }
    }
}
