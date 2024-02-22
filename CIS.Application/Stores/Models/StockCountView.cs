using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Stores.Models
{
    public sealed class StockCountView
    {
        public required int ProductNumber { get; set; }
        public required string ProductName { get; set; }
        public required int Quantity { get; set; }
        public required string CountedByPersonFullName { get; set; }
        public required DateTime CountedDateTime { get; set; }
    }
}
