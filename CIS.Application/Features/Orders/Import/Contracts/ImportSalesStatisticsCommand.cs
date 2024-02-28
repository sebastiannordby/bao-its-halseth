using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Orders.Import.Contracts
{
    public sealed class ImportSalesStatisticsCommand : CISImportCommand
    {
        public required IEnumerable<ImportSalesStatisticsDefinition> Definitions { get; set; }
    }
}
