using CIS.Application.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Products.Migration.Contracts
{
    public sealed class LegacySystemProductSource
    {
        public required IReadOnlyCollection<Vareinfo> Data { get; set; }
    }
}
