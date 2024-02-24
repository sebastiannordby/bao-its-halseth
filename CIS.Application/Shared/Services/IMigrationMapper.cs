using CIS.Application.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Services
{
    public interface IMigrationMapper<TIn, TOut>
        where TOut : CISImportDefinition
    {
        Task<IReadOnlyCollection<TOut>> Map(TIn input, CancellationToken cancellationToken);
    }
}
