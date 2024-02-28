using Bogus;
using CIS.Application.Features.Orders;
using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Migration.Contracts;
using CIS.Application.Legacy;
using CIS.Application.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Tests.Domain.Orders.Migration
{
    public class LegacySystemSalesOrderMapperTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrationMapper<LegacySystemSalesOrderSource, ImportSalesOrderDefinition> _sut;

        public LegacySystemSalesOrderMapperTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddOrderFeature();

            var services = serviceCollection.BuildServiceProvider();

            _sut = services.GetRequiredService<IMigrationMapper<LegacySystemSalesOrderSource, ImportSalesOrderDefinition>>();
        }


        [Fact]
        public async Task MapShouldProduceOutput()
        {
            var bogus = new Faker<Ordre>();
            var record = bogus.Generate();
            var data = new List<Ordre>()
            {
                record
            };

            var input = new LegacySystemSalesOrderSource()
            {
                OrderGrouping = new List<List<Ordre>>()
                {
                    data
                }
            };

            var result = await _sut.Map(input, CancellationToken.None);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task MapShouldProduceSameCount()
        {
            var bogus = new Faker<Ordre>();
            var data = bogus
                .GenerateBetween(1, 200);
            var grouping = data
                .GroupBy(x => new { x.Dato, x.Butikknr });

            var input = new LegacySystemSalesOrderSource()
            {
                OrderGrouping = grouping
            };

            var result = await _sut.Map(input, CancellationToken.None);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(grouping.Count(), result.Count());
        }
    }
}
