using CIS.Application.Products;
using CIS.Application.Products.Import.Contracts;
using CIS.Library.Products.Import;
using CIS.Library.Shared.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Tests.Domain.Products.Import
{
    public class ProcessImportProductCommandServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IProcessImportCommandService<ImportProductCommand> _sut;

        public ProcessImportProductCommandServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddProductFeature();

            fixture.AddLegacyHubMock(serviceCollection);
            
            var services = serviceCollection.BuildServiceProvider();
            _sut = services.GetRequiredService<IProcessImportCommandService<ImportProductCommand>>();
        }

        [Fact]
        public async Task ImportShouldValidateRequest()
        {
            var command = new ImportProductCommand()
            {
                Definitions = new List<ImportProductDefinition>()
            };

            await Assert.ThrowsAsync<ValidationException>(async() =>
            {
                await _sut.Import(command, CancellationToken.None);
            });
        }

        [Fact]
        public async Task ImportShouldValidateDefinitions()
        {
            var command = new ImportProductCommand()
            {
                Definitions = new List<ImportProductDefinition>()
                {
                    new ImportProductDefinition()
                    {
                        CurrencyCode = string.Empty,
                        Name = string.Empty,
                    }
                }
            };

            await Assert.ThrowsAsync<ValidationException>(async () =>
            {
                await _sut.Import(command, CancellationToken.None);
            });
        }
    }
}
