using CIS.Library.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Library.Products.Repositories
{
    public interface IProductViewRepository
    {
        Task<IReadOnlyCollection<ProductView>> List();
    }
}
