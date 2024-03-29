﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Products.Models
{
    public sealed class ProductDao
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public int? AlternateNumber { get; set; }
        public required string Name { get; set; }
        public string? AlternateName { get; set; }
        public string? SuppliersProductNumber { get; set; }
        public required string EAN { get; set; }
        public bool IsActive { get; set; }
        public Guid? ProductGroupId { get; set; }
        public Guid? ProductPriceId { get; set; }
    }
}
