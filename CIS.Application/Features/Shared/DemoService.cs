using CIS.Application.Features.Orders.Infrastructure;
using CIS.Application.Features.Orders.Migration.Infrastructure;
using CIS.Application.Features.Products.Infrastructure;
using CIS.Application.Features.Stores.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Shared
{
    public sealed class DemoService
    {
        private readonly ISalesCommands _salesCommands;
        private readonly IMigrationCommands _migrationCommands;
        private readonly IProductCommands _productCommands;
        private readonly IStoreCommands _storeCommands;

        public DemoService(
            ISalesCommands salesCommands,
            IMigrationCommands migrationCommands,
            IProductCommands productCommands,
            IStoreCommands storeCommands)
        {
            _salesCommands = salesCommands;
            _migrationCommands = migrationCommands;
            _productCommands = productCommands;
            _storeCommands = storeCommands;
        }

        public async Task ResetAllData(CancellationToken cancellationToken)
        {
            await _salesCommands.DeleteAllSalesData(cancellationToken);
            await _migrationCommands.DeleteAllMigrationData(cancellationToken);
            await _productCommands.DeleteAllProductData(cancellationToken);
            await _storeCommands.DeleteAllStoreData(cancellationToken);
        }
    }
}
