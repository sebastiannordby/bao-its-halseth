using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Stores.Models
{
    public sealed class StoreDao
    {
        public int Number { get; set; }
        public required string Name { get; set; }

        public int OwnerCustomerNumber { get; set; }

        public string? AddressLine { get; set; }
        public string? AddressPostalCode { get; set; }
        public string? AddressPostalOffice { get; set; }

        public int? RegionId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
