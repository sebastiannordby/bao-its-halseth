using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Orders.Infrastructure
{
    public interface ISalesQueries
    {
        Task<IReadOnlyCollection<SalesOrderView>> List(int customerNumber, int pageSize, int pageIndex);
        Task<IReadOnlyCollection<SalesOrderView>> List(int pageSize, int pageIndex);
        IQueryable<SalesOrderView> Query();
        Task<IReadOnlyCollection<MostSoldProductView>> GetMostSoldProduct(int count);
        Task<IReadOnlyCollection<StoreMostBoughtView>> GetMostBoughtViews(int count);
        Task<SeasonalityAnalysisResult> AnalyzeSeasonality(int year);
    }
}
