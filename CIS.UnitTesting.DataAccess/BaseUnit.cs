using Microsoft.Extensions.DependencyInjection;

namespace CIS.UnitTesting.DataAccess
{
    public class Tests
    {
        public IServiceProvider Services { get; private set; }

        [SetUp]
        public void Setup()
        {
            Services = ServiceProviderBuilder.BuildServiceProvider((services) =>
            {
                //services.AddOrderServiceDomainLayer();
                //services.AddOrderServiceDataAccessLayer(efOptions =>
                //{
                //    efOptions.UseInMemoryDatabase(nameof(BaseOrderServiceTest), b =>
                //    {
                //        b.EnableNullChecks(false);
                //    });
                //});
            });
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}