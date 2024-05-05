using CIS.Application.Features;
using CIS.Application.Features.Stores;
using CIS.Application.Legacy;
using CIS.Application.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CIS.Tests.Domain.Stores.Migration
{
    public class MigrateLegacyCustomerServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrateLegacyService<Butikkliste> _sut;
        private readonly INotifyClientService _notifyClientService;

        public MigrateLegacyCustomerServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddStoreFeature();

            _notifyClientService = fixture.AddNotifyClientServiceMock(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();
            _sut = services.GetRequiredService<IMigrateLegacyService<Butikkliste>>();
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
