using CIS.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace CIS.UnitTesting.DataAccess
{
    public class DbContextTests : DataAccessTest
    {
        [Test]
        public void ContextShouldBeAccessableDI()
        {
            Services.GetService<CISDbContext>();
        }
    }
}
