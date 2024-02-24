using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Orders;
using CIS.Application.Orders.Contracts.Import;
using CIS.Application.Orders.Migration.Contracts;
using CIS.Application.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Tests.Domain.Orders.Migration
{
    public class MigrateLegacySalgServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrateLegacyService<Salg> _sut;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hubMock;
        private readonly IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition> _mapperMock;

        public MigrateLegacySalgServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();

            serviceCollection.AddOrderFeature();

            _hubMock = fixture.AddLegacyHubMock(serviceCollection);

            _mapperMock = Substitute.For<IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition>>();
            serviceCollection.AddSingleton(_mapperMock);
            
            var services = serviceCollection.BuildServiceProvider();

            _sut = services.GetRequiredService<IMigrateLegacyService<Salg>>();
        }

        [Fact]
        public async Task ShouldSendMessages()
        {
            await _sut.Migrate(CancellationToken.None);
            await _hubMock.Clients.All
                .Received()
                .ReceiveMessage(Arg.Any<string>());
        }
    }
}
