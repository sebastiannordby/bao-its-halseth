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
    public class MigrateLegacyOrderServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrateLegacyService<Ordre> _sut;
        private readonly IMigrationMapper<LegacySystemSalesOrderSource, ImportSalesOrderDefinition> _sutMapper;
        private readonly INotifyClientService _notifyClientService;

        public MigrateLegacyOrderServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddOrderFeature();

            _notifyClientService = fixture.AddNotifyClientServiceMock(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();

            _sut = services.GetRequiredService<IMigrateLegacyService<Ordre>>();
            _sutMapper = services.GetRequiredService<IMigrationMapper<LegacySystemSalesOrderSource, ImportSalesOrderDefinition>>();
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
