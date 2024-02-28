using CIS.Application.Features.Stores.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Stores.Infrastructure
{
    public interface IStoreQueries
    {
        Task<IReadOnlyCollection<CustomerView>> List(
            CancellationToken cancellationToken = default);
    }
}
