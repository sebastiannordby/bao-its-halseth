using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Stores.Infrastructure
{
    public interface IStoreCommands
    {
        Task DeleteAllStoreData(CancellationToken cancellationToken);
    }
}
