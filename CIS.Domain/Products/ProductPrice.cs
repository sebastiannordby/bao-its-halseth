using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Domain.Products
{
    public class ProductPrice
    {
        public int Id { get; set; }

        /// <summary>
        /// What CustomInstall has to pay for the product.
        /// </summary>
        public decimal? CostPrice { get; set; } // our_price

        /// <summary>
        /// What the Store has to pay for a product.
        /// </summary>
        public decimal? PurchasePrice { get; set; }

        /// <summary>
        /// What the Store makes on a product.
        /// </summary>
        public decimal? StorePrice { get; set; } 

        /// <summary>
        /// Currency code in like NOK, SEK, DKK.
        /// </summary>
        public required string CurrencyCode { get; set; } 
    }
}
