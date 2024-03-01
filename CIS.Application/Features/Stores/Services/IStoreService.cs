using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Application.Features.Stores.Models;

namespace CIS.Application.Features.Stores.Services
{
    public interface IStoreService
    {
        Task<IReadOnlyCollection<StoreView>> List(CancellationToken cancellationToken);
    }
}
