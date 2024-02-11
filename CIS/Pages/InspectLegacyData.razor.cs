using CIS.Application.Legacy;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using System.Linq.Dynamic.Core;

namespace CIS.Pages
{
    public partial class InspectLegacyData : ComponentBase
    {
        [Inject]
        public SWNDistroContext LegacyDbContext { get; set; }

        private int _legacyOrdersCount;
        private IEnumerable<Ordre> _legacyOrders;

        private int _legacyStoresCount;
        private IEnumerable<Butikkliste> _legacyStores;

        private int _legacyProductCount;
        private IEnumerable<Vareinfo> _legacyProducts;

        private int _legacySalesStatisticsCount;
        private IEnumerable<Salg> _legacySalesStatistics;

        private async Task LoadLegacySalesStatistics(LoadDataArgs args)
        {
            var query = LegacyDbContext.Salgs
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacySalesStatisticsCount = await query.CountAsync();
            _legacySalesStatistics = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync();
        }

        private async Task LoadLegacyOrders(LoadDataArgs args)
        {
            var query = LegacyDbContext.Ordres
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacyOrdersCount = await query.CountAsync();
            _legacyOrders = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync();
        }

        private async Task LoadLegacyStores(LoadDataArgs args)
        {
            var query = LegacyDbContext.Butikklistes
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacyStoresCount = await query.CountAsync();
            _legacyStores = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync();
        }

        private async Task LoadLegacyProducts(LoadDataArgs args)
        {
            var query = LegacyDbContext.Vareinfos
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacyProductCount = await query.CountAsync();
            _legacyProducts = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync();
        }
    }
}
