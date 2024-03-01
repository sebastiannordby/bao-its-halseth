using CIS.Application.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Repositories
{
    public interface IMigrationTaskRepo
    {
        Task<IEnumerable<MigrationTask>> GetMigrationTasks(CancellationToken cancellationToken);
        Task Complete(MigrationTask.TaskType taskType, CancellationToken cancellationToken);
    }
}
