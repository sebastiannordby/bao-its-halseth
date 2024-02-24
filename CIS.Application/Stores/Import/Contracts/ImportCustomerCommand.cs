using CIS.Application.Shared.Contracts;
using CIS.Application.Stores.Models.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Stores.Import.Contracts
{
    public sealed class ImportCustomerCommand : CISImportCommand
    {
        public required IReadOnlyCollection<ImportCustomerDefinition> Definitions { get; set; }
    }
}
