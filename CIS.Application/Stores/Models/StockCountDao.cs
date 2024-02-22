using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Stores.Models
{
    public class StockCountDao
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid StoreId { get; set; }
        public int Quantity { get; set; }
        public required string CountedByPersonFullName { get; set; }
        public DateTime CountedDateTime { get; set; }
    }
}
