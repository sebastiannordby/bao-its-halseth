using CIS.Application.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Stores.Migration.Contracts
{
    public sealed class LegacySystemCustomerSource
    {
        public required IReadOnlyCollection<Butikkliste> Data { get; set; }
    }
}
