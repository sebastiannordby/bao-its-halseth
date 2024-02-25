using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using CIS.Application;

namespace CIS.Tests.Integration
{
    public class IntegrationTestFixture : IAsyncLifetime
    {
        private MsSqlContainer? _sqlServerContainer;
        private ServiceProvider _services;
        public ServiceProvider Services => _services;

        public async Task DisposeAsync()
        {
            if(_sqlServerContainer is not null)
            {
                await _sqlServerContainer.DisposeAsync();
            } 
        }

        public async Task InitializeAsync()
        {
            _sqlServerContainer = new MsSqlBuilder()
                .Build();

            await _sqlServerContainer.StartAsync();
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddCISApplication(
                _sqlServerContainer.GetConnectionString());

            var services = serviceCollection
                .BuildServiceProvider();

            await services.ApplyCISDataMigrations(
                requiresMigrationFromLegacy: false,
                insertTestUser: true);

            _services = services;
        }
    }
}
