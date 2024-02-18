using CIS.Application.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Import.Contracts
{
    public sealed class LegacyCISOrderSource
    {
        public required IEnumerable<IEnumerable<Ordre>> OrderGrouping { get; set; }
    }
}
