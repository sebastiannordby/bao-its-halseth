﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Orders.Infrastructure.Models
{
    public class SalesOrderLineDao
    {
        public Guid Id { get; set; }
        public Guid SalesOrderId { get; set; }
        public int ProductNumber { get; set; }
        public required string ProductName { get; set; }
        public string? EAN { get; set; }
        public int Quantity { get; set; }
        public int QuantityDelivered { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? StorePrice { get; set; }
        public required string CurrencyCode { get; set; }
    }
}
