using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Domain.Products.Models
{
    public class Product
    {
        public int Number { get; set; }
        public required string Name { get; set; }
        public string? EAN { get; set; }
        public string? AlternateName { get; set; }
        public int? ProductGroupId { get; set; }
        public int? ProductPriceId { get; set; }
        public bool IsActive { get; set; }
    }
}
