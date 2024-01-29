using CIS.Domain.Products.Models.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Domain.Products.Services
{
    public interface IProductService
    {
        Task<bool> Import(IEnumerable<ProductImportDefinition> importDefinitions);
    }
}
