using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Orders.Migration.Infrastructure
{
    public interface IMigrationCommands
    {
        Task DeleteAllMigrationData(CancellationToken cancellationToken);
    }
}
