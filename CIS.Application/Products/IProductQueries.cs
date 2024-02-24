using CIS.Library.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Products
{
    public interface IProductQueries
    {
        Task<IReadOnlyCollection<ProductView>> List(
            CancellationToken cancellationToken = default);
    }
}
