using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Domain.Orders.Models
{
    public sealed class SalesOrder
    {
        public int Number { get; set; }
        public int AlternateNumber { get; set; } // ordrenrNett
        public DateOnly OrderDate { get; set; } // dato
        public string? Reference { get; set; } // orderref

        public DateOnly DeliveredDate { get; set; }

        public int StoreNumber { get; set; }
        public required string StoreName { get; set; }

        public int CustomerNumber { get; set; }
        public required string CustomerName { get; set; }

        public bool IsDeleted { get; set; }

        private class Line
        {
            public int ProductNumber { get; set; }
            public required string ProductName { get; set; } 
            public required string EAN { get; set; }
            public decimal Quantity { get; set; }
            public decimal QuantityDelivered { get; set; }
            public decimal? CostPrice { get; set; }
            public decimal? PurchasePrice { get; set; }
            public decimal? StorePrice { get; set; }
            public required string CurrencyCode { get; set; }
        }
    }
}
