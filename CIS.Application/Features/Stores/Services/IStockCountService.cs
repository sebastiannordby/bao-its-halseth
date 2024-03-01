using CIS.Application.Features.Stores.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Stores.Services
{
    public interface IStockCountService
    {
        Task AddStockCount(Guid productId, Guid storeId, int quantity, string countedByPersonFullName, CancellationToken token);
        Task<IReadOnlyCollection<StockCountView>> GetByStore(Guid storeId, CancellationToken token);
        Task<IReadOnlyCollection<StockCountView>> GetHistoryByStore(Guid storeId, CancellationToken token);
    }
}
