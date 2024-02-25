using CIS.Application;
using CIS.Application.Stores.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CIS.Tests.Integration
{
    [CollectionDefinition(nameof(IntegrationTestCollection))]
    public class DummyTest : IClassFixture<IntegrationTestFixture>
    {
        private readonly CISDbContext _dbContext;

        public DummyTest(IntegrationTestFixture fixture)
        {
            _dbContext = fixture.Services.GetRequiredService<CISDbContext>();
        }

        [Fact]
        public async Task CheckToSeeIfIntegrationTestWillFailInPipeline()
        {
            var customer = new CustomerDao()
            {
                Name = "Hello"
            };

            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            Assert.NotEqual(Guid.Empty, customer.Id);
        }
    }
}