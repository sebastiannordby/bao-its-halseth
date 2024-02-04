using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Library.Stores.Models
{
    public class StoreView
    {
        public int Number { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }
        public int CustomerNumber { get; set; }
        public required string CustomerName { get; set; }
        public string? CustomerContactPersonName { get; set; }
        public string? CustomerContactPersonEmailAddress { get; set; }
        public string? CustomerContactPersonPhoneNumber { get; set; }
        public int? CustomerCustomerGroupNumber { get; set; }
        public string? CustomerGroupName { get; set; }
        public string ? StreetAddress { get; set; }
        public string? PostalName{ get; set; }
        public string? PostalCode { get; set; }
        public string? Location { get; set; }
    }

}
