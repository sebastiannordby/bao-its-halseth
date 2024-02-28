using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Stores.Models
{
    public sealed class StoreDao
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public required string Name { get; set; }

        public Guid OwnerCustomerId { get; set; }

        public string? AddressLine { get; set; }
        public string? AddressPostalCode { get; set; }
        public string? AddressPostalOffice { get; set; }

        public Guid? RegionId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
