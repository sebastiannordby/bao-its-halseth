using CIS.Application.Orders.Contracts;
using CIS.Application.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Import.Contracts
{
    public sealed class ImportSalesOrderCommand : CISImportCommand
    {
        public required IEnumerable<ImportSalesOrderDefinition> Definitions { get; set; }
    }
}
