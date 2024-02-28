using Bogus;
using CIS.Application.Features.Products;
using CIS.Application.Features.Products.Migration.Contracts;
using CIS.Application.Features.Products.Models.Import;
using CIS.Application.Legacy;
using CIS.Application.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Tests.Domain.Products.Migration
{
    public class LegacySystemProductMapperTests : IClassFixture<DomainTestFixture>
    {
        private readonly IMigrationMapper<LegacySystemProductSource, ImportProductDefinition> _sut;

        public LegacySystemProductMapperTests(DomainTestFixture fixture)
        {
            var serviceColletion = fixture.GetServiceCollection();
            serviceColletion.AddProductFeature();

            var services = serviceColletion.BuildServiceProvider(); ;
            _sut = services.GetRequiredService<IMigrationMapper<LegacySystemProductSource, ImportProductDefinition>>();
        }


        [Fact]
        public async Task MapShouldProduceOutput()
        {
            var bogus = new Faker<Vareinfo>();
            var record = bogus.Generate();
            var data = new List<Vareinfo>()
            {
                record
            };

            var input = new LegacySystemProductSource()
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
            var bogus = new Faker<Vareinfo>();
            var data = bogus
                .GenerateBetween(1, 200);

            var input = new LegacySystemProductSource()
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
