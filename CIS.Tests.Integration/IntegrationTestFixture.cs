using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using CIS.Application;
using CIS.Application.Legacy;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CIS.Tests.Integration
{
    public class IntegrationTestFixture : IAsyncLifetime
    {
        private MsSqlContainer? _cisSQLServerContainer;
        private MsSqlContainer? _swnSQLServerContainer;
        private ServiceProvider _services;
        public ServiceProvider Services => _services;

        public async Task DisposeAsync()
        {
            if(_cisSQLServerContainer is not null)
            {
                await _cisSQLServerContainer.DisposeAsync();
            } 

            if(_swnSQLServerContainer is not null)
            {
                await _swnSQLServerContainer.DisposeAsync();
            }
        }

        public async Task InitializeAsync()
        {
            _swnSQLServerContainer = new MsSqlBuilder()
                .Build();

            _cisSQLServerContainer = new MsSqlBuilder()
                 .Build();

            await _swnSQLServerContainer.StartAsync();
            await _cisSQLServerContainer.StartAsync();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddCISApplication(
                _cisSQLServerContainer.GetConnectionString());

            serviceCollection.AddSWNDistroLegacyDatabase(
                _swnSQLServerContainer.GetConnectionString());

            var services = serviceCollection
                .BuildServiceProvider();

            await services.ApplyCISDataMigrations(
                requiresMigrationFromLegacy: false,
                insertTestUser: true);

            _services = services;

            SWNDistroSeedData.SeedWithIntegrationTestData(services);
        }
    }
}
