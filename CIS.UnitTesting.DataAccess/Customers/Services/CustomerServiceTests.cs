using CIS.Domain.Customers.Services;
using CIS.Library.Customers.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.UnitTesting.DataAccess.Customers.Services
{
    public sealed class CustomerServiceTests : DataAccessTest
    {
        [Test]
        public void ServicesAvailableInDI()
        {
            var customerService = Services.GetRequiredService<ICustomerService>();

            Assert.IsNotNull(customerService);
        }
    }
}
