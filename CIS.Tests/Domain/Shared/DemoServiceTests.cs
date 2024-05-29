using CIS.Application.Features.Orders.Infrastructure;
using CIS.Application.Features.Orders.Migration.Infrastructure;
using CIS.Application.Features.Products.Infrastructure;
using CIS.Application.Features.Shared;
using CIS.Application.Features.Stores.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Tests.Domain.Shared
{
    public sealed class DemoServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly ISalesCommands _salesCommandsMock;
        private readonly IMigrationCommands _migrationCommandsMock;
        private readonly IProductCommands _productCommands;
        private readonly IStoreCommands _storeCommands;
        private readonly DemoService _sut;

        public DemoServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddSharedFeature();

            _salesCommandsMock = Substitute.For<ISalesCommands>();
            serviceCollection.AddSingleton(_salesCommandsMock);

            _migrationCommandsMock = Substitute.For<IMigrationCommands>();
            serviceCollection.AddSingleton(_migrationCommandsMock);

            _productCommands = Substitute.For<IProductCommands>();
            serviceCollection.AddSingleton(_productCommands);

            _storeCommands = Substitute.For<IStoreCommands>();
            serviceCollection.AddSingleton(_storeCommands);

            var services = serviceCollection.BuildServiceProvider();

            _sut = services.GetRequiredService<DemoService>();
        }

        [Fact]
        public async Task ResetAllDataShouldCallDatabase()
        {
            // When
            await _sut.ResetAllData(CancellationToken.None);

            // Then
            await _salesCommandsMock
                .Received(1)
                .DeleteAllSalesData(Arg.Any<CancellationToken>());

            await _migrationCommandsMock
                .Received(1)
                .DeleteAllMigrationData(Arg.Any<CancellationToken>());

            await _productCommands
                .Received(1)
                .DeleteAllProductData(Arg.Any<CancellationToken>());

            await _storeCommands
                .Received(1)
                .DeleteAllStoreData(Arg.Any<CancellationToken>());
        }
    }
}
