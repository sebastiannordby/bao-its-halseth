using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Library.Products.Import
{
    public class ProductImportDefinition
    {
        public int Number { get; set; }
        public required string Name { get; set; }
        public string? AlternateName { get; set; }
        public int? ProductGroupId { get; set; }

        public decimal? CostPrice { get; set; } 
        public decimal? PurchasePrice { get; set; }
        public decimal? StorePrice { get; set; }
        public required string CurrencyCode { get; set; }
    }
}
