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
        Task AddStockCount(Guid productId, Guid storeId, int quantity, string countedByPersonFullName);
        Task<IReadOnlyCollection<StockCountView>> GetByStore(Guid storeId);
        Task<IReadOnlyCollection<StockCountView>> GetHistoryByStore(Guid storeId);
    }
}
