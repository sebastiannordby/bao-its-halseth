//using CIS.Application;
//using CIS.Application.Customers.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CIS.UnitTesting.Application.Services
//{
//    internal class CustomerDummyDataService
//    {
//        private readonly CISDbContext _dbContext;

//        public CustomerDummyDataService(CISDbContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        public async Task Insert()
//        {
//            await _dbContext.Customers.AddAsync(new CustomerDao()
//            {
//                Number = 1,
//                Name = "Seven11 - Oslo Lufthavn",
//                ContactPersonEmailAddress = "post@sebastiannordby.no",
//                ContactPersonName = "Sebastian Nordby",
//                ContactPersonPhoneNumber = "1234567890",
//                CustomerGroupId = null,
//                IsActive = true
//            });
//            await _dbContext.SaveChangesAsync();
//        }
//    }
//}
