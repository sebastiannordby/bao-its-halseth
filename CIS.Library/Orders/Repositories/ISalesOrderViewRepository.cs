using CIS.Library.Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Library.Orders.Repositories
{
    public interface ISalesOrderViewRepository
    {
        Task<IReadOnlyCollection<SalesOrderView>> List(
            int pageSize, int pageIndex);
    }
}
