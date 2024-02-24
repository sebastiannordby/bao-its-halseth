using CIS.Application.Hubs;
using CIS.Application.Legacy;
using CIS.Application.Listeners;
using CIS.Application.Products;
using CIS.Application.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Tests.Domain.Products.Migration
{
    public class MigrateLegacyProductServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrateLegacyService<Vareinfo> _sut;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hubMock;

        public MigrateLegacyProductServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddProductFeature();

            _hubMock = fixture.AddLegacyHubMock(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();
            _sut = services.GetRequiredService<IMigrateLegacyService<Vareinfo>>();
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
