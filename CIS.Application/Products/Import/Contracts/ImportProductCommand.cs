using CIS.Application.Shared.Contracts;
using CIS.Library.Products.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Products.Import.Contracts
{
    public sealed class ImportProductCommand : CISImportCommand
    {
        public required IReadOnlyCollection<ImportProductDefinition> Definitions { get; set; }
    }
}
