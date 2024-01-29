using CIS.DataAccess;
using CIS.UnitTesting.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CIS.UnitTesting.DataAccess
{
    public abstract class DataAccessTest
    {
        public IServiceProvider Services { get; private set; }

        [SetUp]
        public void Setup()
        {
            Services = ServiceProviderBuilder.BuildServiceProvider((services) =>
            {
                services.AddDataAccess(efOptions =>
                {
                    efOptions.UseInMemoryDatabase(nameof(DataAccessTest), b => {
                        b.EnableNullChecks(false);
                    });
                });

                services.AddScoped<CustomerDummyDataService>();
            });
        }
    }
}