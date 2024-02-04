using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Customers.Models
{
    public sealed class CustomerGroupDao
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public required string Name { get; set; }
    }
}
