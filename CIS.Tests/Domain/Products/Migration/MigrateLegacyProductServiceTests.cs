using CIS.Application.Features;
using CIS.Application.Features.Products;
using CIS.Application.Legacy;
using CIS.Application.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CIS.Tests.Domain.Products.Migration
{
    public class MigrateLegacyProductServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrateLegacyService<Vareinfo> _sut;
        private readonly INotifyClientService _notifyClientService;

        public MigrateLegacyProductServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddProductFeature();

            _notifyClientService = fixture.AddNotifyClientServiceMock(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();
            _sut = services.GetRequiredService<IMigrateLegacyService<Vareinfo>>();
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
