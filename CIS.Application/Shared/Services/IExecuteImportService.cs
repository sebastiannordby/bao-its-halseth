using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Library.Shared.Services
{
    public interface IExecuteImportService<TCommand>
    {
        Task<bool> Import(IEnumerable<TCommand> command);
    }

    public interface IProcessImportCommandService<TCommand>
    {
        Task<bool> Import(TCommand command, CancellationToken cancellationToken);
    }
}
