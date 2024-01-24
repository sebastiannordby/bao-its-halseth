using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Domain.Models
{
    public class Store
    {
        public int Number { get; set; }
        public string Name { get; set; }

        public Guid CustomerNumber { get; set; }

        public string AddressPostalOffice { get; set; }
        public string AddressPostalCode { get; set;}
        public Guid? RegionId { get; set; }
        public bool IsActive { get; set; }
    }
}
