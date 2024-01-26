using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Domain.Customers.Models
{
    public class CustomerGroup
    {
        public int Number { get; set; }
        public required string Name { get; set; }
    }
}
