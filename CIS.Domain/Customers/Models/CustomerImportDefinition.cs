using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Domain.Customers.Models
{
    public class CustomerImportDefinition
    {
        public required int Number { get; set; }
        public required string Name { get; set; }
        public required string? ContactPersonName { get; set; }
        public required string? ContactPersonEmailAddress { get; set; }
        public required string? ContactPersonPhoneNumber { get; set; }
        public required int? CustomerGroupNumber { get; set; }
        public required bool IsActive { get; set; }

        public StoreDefinition? Store { get; set; }

        public class StoreDefinition
        {
            public required int? Number { get; set; }
            public required string Name { get; set; }
            public required string? AddressLine { get; set; }
            public required string? AddressPostalCode { get; set; }
            public required string? AddressPostalOffice { get; set; }
        }
    }
}
