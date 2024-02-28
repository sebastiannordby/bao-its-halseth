using CIS.Application.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Orders.Migration.Contracts
{
    public sealed class LegacySystemSalesOrderSource
    {
        public required IEnumerable<IEnumerable<Ordre>> OrderGrouping { get; set; }
    }
}
