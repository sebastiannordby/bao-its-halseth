using CIS.Application.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Contracts
{
    public class ImportSalesOrderDefinition : CISImportDefinition
    {
        public int Number { get; set; }
        public string? AlternateNumber { get; set; } // ordrenrNett
        public DateOnly OrderDate { get; set; } // dato
        public string? Reference { get; set; } // orderref

        public DateOnly DeliveredDate { get; set; }

        public int StoreNumber { get; set; }
        public required string StoreName { get; set; }

        public int CustomerNumber { get; set; }
        public required string CustomerName { get; set; }

        public bool IsDeleted { get; set; }

        public List<Line> Lines { get; set; } = new List<Line>();

        public class Line
        {
            public int ProductNumber { get; set; } // vareID
            public required string ProductName { get; set; }
            public required string EAN { get; set; }
            public decimal Quantity { get; set; } // antall
            public decimal QuantityDelivered { get; set; } // antallLevert
            public decimal? CostPrice { get; set; } // our_price
            public decimal? PurchasePrice { get; set; } // innpris
            public decimal? StorePrice { get; set; } // 
            public required string CurrencyCode { get; set; } = "NOK";
        }
    }
}
