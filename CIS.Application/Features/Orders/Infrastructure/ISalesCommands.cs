using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Orders.Infrastructure
{
    public interface ISalesCommands
    {
        Task DeleteAllSalesData(CancellationToken cancellationToken);
    }
}
