using CIS.Application.Features.Products;
using CIS.Application.Features.Products.Import.Contracts;
using CIS.Application.Features.Products.Models.Import;
using CIS.Library.Shared.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CIS.Tests.Domain.Products.Import
{
    public class ProcessImportProductCommandServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IProcessImportCommandService<ImportProductCommand> _sut;

        public ProcessImportProductCommandServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();
            serviceCollection.AddProductFeature();

            fixture.AddNotifyClientServiceMock(serviceCollection);

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

            await Assert.ThrowsAsync<ValidationException>(async () =>
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
