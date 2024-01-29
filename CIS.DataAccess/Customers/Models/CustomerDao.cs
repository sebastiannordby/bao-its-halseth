using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Customers.Models
{
    public sealed class CustomerDao
    {
        public int Number { get; set; }
        public required string Name { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonEmailAddress { get; set; }
        public string? ContactPersonPhoneNumber { get; set; }
        public int? CustomerGroupNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
