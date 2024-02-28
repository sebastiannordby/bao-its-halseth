using CIS.Application.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Orders.Contracts
{
    public class ImportSalesStatisticsDefinition : CISImportDefinition
    {
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public int StoreNumber { get; set; }
        public int ProductNumber { get; set; }
        public int CustomerNumber { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal StorePrice { get; set; }
    }
}
