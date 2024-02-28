using CIS.Application.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Products.Models.Import
{
    public class ImportProductDefinition : CISImportDefinition
    {
        public int Number { get; set; }
        public int AlternateNumber { get; set; }
        public required string Name { get; set; }
        public string? AlternateName { get; set; }
        public string? SuppliersProductNumber { get; set; }
        public string? EAN { get; set; }
        public bool IsActive { get; set; }
        public int? ProductGroupNumber { get; set; }
        public string? ProductGroupName { get; set; }

        public decimal? CostPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? StorePrice { get; set; }
        public required string CurrencyCode { get; set; }
    }
}
