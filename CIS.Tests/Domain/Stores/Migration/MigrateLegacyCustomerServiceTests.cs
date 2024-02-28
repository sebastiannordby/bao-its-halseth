using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Application.Features.Stores;

namespace CIS.Tests.Domain.Stores.Migration
{
    public class MigrateLegacyCustomerServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrateLegacyService<Butikkliste> _sut;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hubMock;

        public MigrateLegacyCustomerServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddStoreFeature();

            _hubMock = fixture.AddLegacyHubMock(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();
            _sut = services.GetRequiredService<IMigrateLegacyService<Butikkliste>>();
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
