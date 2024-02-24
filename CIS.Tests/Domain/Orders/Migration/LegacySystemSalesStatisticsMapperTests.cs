using Bogus;
using CIS.Application.Legacy;
using CIS.Application.Orders;
using CIS.Application.Orders.Contracts.Import;
using CIS.Application.Orders.Migration.Contracts;
using CIS.Application.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Tests.Domain.Orders.Migration
{
    public class LegacySystemSalesStatisticsMapperTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition> _sut;

        public LegacySystemSalesStatisticsMapperTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddOrderFeature();

            var services = serviceCollection.BuildServiceProvider();

            _sut = services.GetRequiredService<IMigrationMapper<LegacySystemSalesStatisticsSource, ImportSalesStatisticsDefinition>>();
        }

        [Fact]
        public async Task MapShouldProduceOutput()
        {
            var bogus = new Faker<Salg>();
            var record = bogus.Generate();
            var data = new List<Salg>() 
            { 
                record
            };

            var input = new LegacySystemSalesStatisticsSource()
            {
                Data = data
            };

            var result = await _sut.Map(input, CancellationToken.None);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task MapShouldProduceSameCount()
        {
            var bogus = new Faker<Salg>();
            var data = bogus
                .GenerateBetween(1, 200);

            var input = new LegacySystemSalesStatisticsSource()
            {
                Data = data
            };

            var result = await _sut.Map(input, CancellationToken.None);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(data.Count, result.Count());
        }
    }
}
