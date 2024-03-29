﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Products.Models
{
    public record ProductView(
        Guid Id,
        int Number,
        string Name,
        string? AlternateName,
        decimal? CostPrice,
        decimal? PurchasePrice,
        decimal? StorePrice,
        int? ProductGroupNumber,
        string? ProductGroupName
    );
}
