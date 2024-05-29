using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Products.Infrastructure
{
    public interface IProductCommands
    {
        Task DeleteAllProductData(CancellationToken cancellationToken);
    }
}
