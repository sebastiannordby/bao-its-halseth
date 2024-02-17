using CIS.Application.Orders.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Import.Contracts
{
    public sealed class ImportSalesOrderCommand
    {
        public required IEnumerable<SalesOrderImportDefinition> Definitions { get; set; }
    }
}
