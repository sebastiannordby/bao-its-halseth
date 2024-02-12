//using CIS.Library.Customers.Repositories;
//using CIS.UnitTesting.Application.Services;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CIS.UnitTesting.Application.Customers.Repositories
//{
//    public sealed class CustomerViewRepositoryTests : DataAccessTest
//    {
//        [Test]
//        public void ServicesAvailableInDI()
//        {
//            var customerViewRepository = Services.GetRequiredService<ICustomerViewRepository>();

//            Assert.IsNotNull(customerViewRepository);
//        }

//        [Test]
//        public async Task GetListOfCustomersEmptyNotNull()
//        {
//            var customerViewRepository = Services.GetRequiredService<ICustomerViewRepository>();
//            var customerList = await customerViewRepository.List();

//            Assert.IsNotNull(customerList);
//            Assert.IsEmpty(customerList);
//        }

//        [Test]
//        public async Task GetListOfCustomersNotNullOrEmpty()
//        {
//            var customerViewRepository = Services.GetRequiredService<ICustomerViewRepository>();
//            var dummyDataService = Services.GetRequiredService<CustomerDummyDataService>();

//            await dummyDataService.Insert();

//            var customerList = await customerViewRepository.List();

//            Assert.IsNotNull(customerList);
//            Assert.IsNotEmpty(customerList);
//        }
//    }
//}
