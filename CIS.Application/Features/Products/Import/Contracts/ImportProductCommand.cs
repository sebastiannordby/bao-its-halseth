using CIS.Application.Features.Products.Models.Import;
using CIS.Application.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Products.Import.Contracts
{
    public sealed class ImportProductCommand : CISImportCommand
    {
        public required IReadOnlyCollection<ImportProductDefinition> Definitions { get; set; }
    }
}
