using CIS.Library.Stores.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.UnitTesting.Application.Stores.Repositories
{
    public class StoreRepositoryTests : DataAccessTest
    {
        [Test]
        public void TestAvailabilityInDI()
        {
            var repo = Services.GetRequiredService<IStoreViewRepository>();

            Assert.IsNotNull(repo);
        }
    }
}
