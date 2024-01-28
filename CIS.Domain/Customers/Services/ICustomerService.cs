using CIS.Domain.Customers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Domain.Customers.Services
{
    public interface ICustomerService
    {
        Task<bool> Import(Customer customer);
    }
}
