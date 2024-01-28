using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Domain.Customers.Models
{
    public sealed class Customer
    {
        public int Number { get; internal set; }
        public required string Name { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonEmailAddress { get; set; }
        public string? ContactPersonPhoneNumber { get; set; }
        public int? CustomerGroupNumber { get; set; }
        public bool IsActive { get; internal set; }

        public static async Task<Customer> Initialize(
            int number,
            string name,
            string? contactPersonName,
            string? contactPersonEmailAddress,
            string? contactPersonPhoneNumber,
            int? customerGroupNumber,
            bool isActive)
        {
            return await Task.FromResult(new Customer()
            {
                Number = number,
                Name = name,
                ContactPersonEmailAddress= contactPersonEmailAddress,
                ContactPersonPhoneNumber = contactPersonPhoneNumber,
                ContactPersonName = contactPersonName,
                CustomerGroupNumber = customerGroupNumber,
                IsActive = isActive
            });
        }
    }
}
