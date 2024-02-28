using CIS.Application.Features.Stores.Models.Import;
using CIS.Application.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Stores.Import.Contracts
{
    public sealed class ImportCustomerCommand : CISImportCommand
    {
        public required IReadOnlyCollection<ImportCustomerDefinition> Definitions { get; set; }
    }
}
