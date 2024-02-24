using CIS.Application.Legacy;
using CIS.Application.Orders.Contracts;
using CIS.Library.Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Infrastructure
{
    public interface ISalesQueries
    {
        Task<IReadOnlyCollection<SalesOrderView>> List(int pageSize, int pageIndex);
        IQueryable<SalesOrderView> Query();
        Task<IReadOnlyCollection<MostSoldProductView>> GetMostSoldProduct(int count);
        Task<IReadOnlyCollection<StoreMostBoughtView>> GetMostBoughtViews(int count);
        Task<SeasonalityAnalysisResult> AnalyzeSeasonality(int year);
    }
}
