using CIS.Application.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Library.Shared.Services
{
    public interface IExecuteImportService<TDefinition>
        where TDefinition : CISImportDefinition
    {
        Task<bool> Import(IEnumerable<TDefinition> command);
    }
}
