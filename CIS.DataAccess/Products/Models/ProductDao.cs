using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Products.Models
{
    public sealed class ProductDao
    {
        public int Number { get; set; }
        public required string Name { get; set; }
        public string? AlternateName { get; set; }
        public int? ProductGroupId { get; set; }
        public int? ProductPriceId { get; set; }
    }
}
