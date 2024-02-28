using Bogus;
using CIS.Application.Legacy;
using CIS.Application.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Application.Features.Stores;
using CIS.Application.Features.Stores.Migration.Contracts;
using CIS.Application.Features.Stores.Models.Import;

namespace CIS.Tests.Domain.Stores.Migration
{
    public class LegacySystemCustomerMapperTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrationMapper<LegacySystemCustomerSource, ImportCustomerDefinition> _sut;

        public LegacySystemCustomerMapperTests(DomainTestFixture fixture)
        {
            var serviceColletion = fixture.GetServiceCollection();
            serviceColletion.AddStoreFeature();

            var services = serviceColletion.BuildServiceProvider(); ;
            _sut = services.GetRequiredService<IMigrationMapper<LegacySystemCustomerSource, ImportCustomerDefinition>>();
        }


        [Fact]
        public async Task MapShouldProduceOutput()
        {
            var bogus = new Faker<Butikkliste>();
            var record = bogus.Generate();
            var data = new List<Butikkliste>()
            {
                record
            };

            var input = new LegacySystemCustomerSource()
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
            var bogus = new Faker<Butikkliste>();
            var data = bogus
                .GenerateBetween(1, 200);

            var input = new LegacySystemCustomerSource()
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
