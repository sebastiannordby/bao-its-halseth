using CIS.Library.Customers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Library.Customers.Repositories
{
    public interface ICustomerViewRepository
    {
        Task<IReadOnlyCollection<CustomerView>> List();
    }
}
