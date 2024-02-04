using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Products.Models
{
    public sealed class ProductPriceDao
    {
        public Guid Id { get; set; }
        public decimal? CostPrice { get; set; } // our_price
        public decimal? PurchasePrice { get; set; }
        public decimal? StorePrice { get; set; }
        public required string CurrencyCode { get; set; }
    }
}
