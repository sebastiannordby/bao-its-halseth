using CIS.Application.Orders.Import.Contracts;
using CIS.Application.Shared.Services;
using CIS.Library.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Application.Orders;
using CIS.Application.Listeners;
using NSubstitute;
using Microsoft.AspNetCore.SignalR;
using CIS.Application.Hubs;
using CIS.Application.Legacy;

namespace CIS.Tests.Domain.Orders
{
    public class MigrateLegacyOrderServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrateLegacyService<Ordre> _sut;
        private readonly IHubContext<ImportLegacyDataHub, IListenImportClient> _hubMock;

        public MigrateLegacyOrderServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddOrderServices();

            _hubMock = Substitute.For<IHubContext<ImportLegacyDataHub, IListenImportClient>> ();
            serviceCollection.AddSingleton(_hubMock);

            var services = serviceCollection.BuildServiceProvider();

            _sut = services.GetRequiredService<IMigrateLegacyService<Ordre>>();
        }

        [Fact]
        public async Task ShouldSendMessages()
        {
            await _sut.Migrate();

            await _hubMock.Clients.All
                .Received()
                .ReceiveMessage(Arg.Any<string>());
        }
    }
}
