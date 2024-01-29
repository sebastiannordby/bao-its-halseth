using CIS.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
