using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Library.Services
{
    public interface IExecuteImportService<TDefinition>
    {
        Task<bool> Import(IEnumerable<TDefinition> definitions);
    }
}
