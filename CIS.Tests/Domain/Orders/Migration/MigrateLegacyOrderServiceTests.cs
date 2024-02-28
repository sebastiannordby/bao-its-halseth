using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Application.Listeners;
using NSubstitute;
using Microsoft.AspNetCore.SignalR;
using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Features.Orders;
using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Migration.Contracts;

namespace CIS.Tests.Domain.Orders.Migration
{
    public class MigrateLegacyOrderServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrateLegacyService<Ordre> _sut;
        private readonly IMigrationMapper<LegacySystemSalesOrderSource, ImportSalesOrderDefinition> _sutMapper;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hubMock;

        public MigrateLegacyOrderServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddOrderFeature();

            _hubMock = fixture.AddLegacyHubMock(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();

            _sut = services.GetRequiredService<IMigrateLegacyService<Ordre>>();
            _sutMapper = services.GetRequiredService<IMigrationMapper<LegacySystemSalesOrderSource, ImportSalesOrderDefinition>>();
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
