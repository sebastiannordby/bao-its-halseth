using CIS.Application;
using CIS.Application.Features.Stores.Models;
using CIS.Application.Legacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CIS.Tests.Integration
{
    [CollectionDefinition(nameof(IntegrationTestCollection))]
    public class DummyTest : IClassFixture<IntegrationTestFixture>
    {
        private readonly CISDbContext _dbContext;
        private readonly SWNDistroContext _distroDbContext;

        public DummyTest(IntegrationTestFixture fixture)
        {
            _dbContext = fixture.Services.GetRequiredService<CISDbContext>();
            _distroDbContext = fixture.Services.GetRequiredService<SWNDistroContext>();
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

        [Fact]
        public async Task SWNDistroHasMoney()
        {
            var test = await _distroDbContext.Butikklistes
                .AsNoTracking()
                .ToListAsync();

            Assert.NotEmpty(test);
        }
    }
}