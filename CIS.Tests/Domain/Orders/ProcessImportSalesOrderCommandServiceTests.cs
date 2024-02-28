using CIS.Library.Shared.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Application.Features.Orders;
using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Import.Contracts;

namespace CIS.Tests.Domain.Orders
{
    public class ProcessImportSalesOrderCommandServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly IProcessImportCommandService<ImportSalesOrderCommand> _sut;

        public ProcessImportSalesOrderCommandServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();

            serviceCollection.AddOrderFeature();

            var services = serviceCollection.BuildServiceProvider();

            _sut = services.GetRequiredService<IProcessImportCommandService<ImportSalesOrderCommand>>();
        }

        [Fact]
        public async Task ImportShouldValidateRequest()
        {
            var command = new ImportSalesOrderCommand()
            {
                Definitions = Enumerable.Empty<ImportSalesOrderDefinition>()
            };

            await Assert.ThrowsAnyAsync<ValidationException>(async() =>
            {
                await _sut.Import(command, CancellationToken.None);
            });
        }

        [Fact]
        public async Task ImportShouldValidateRequestAndContent()
        {
            var command = new ImportSalesOrderCommand()
            {
                Definitions = new List<ImportSalesOrderDefinition>()
                {
                    Activator.CreateInstance<ImportSalesOrderDefinition>()
                }
            };

            await Assert.ThrowsAnyAsync<ValidationException>(async () =>
            {
                await _sut.Import(command, CancellationToken.None);
            });
        }
    }
}
