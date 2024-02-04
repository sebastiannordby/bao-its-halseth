using CIS.Library.Orders.Models.Import;
using CIS.Library.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Orders.Services
{
    internal class ImportOrderService : IExecuteImportService<SalesOrderImportDefinition>
    {
        public Task<bool> Import(IEnumerable<SalesOrderImportDefinition> definitions)
        {
            throw new NotImplementedException();
        }
    }
}
