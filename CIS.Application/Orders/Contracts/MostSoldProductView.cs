using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Contracts
{
    public class MostSoldProductView
    {
        public int ProductNumber { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalSoldFor { get; set; }
        public decimal TotalCostPrice { get; set; }
        public decimal Profit => TotalSoldFor / TotalCostPrice;
    }
}
