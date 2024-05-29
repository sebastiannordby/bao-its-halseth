using CIS.Application.Legacy;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using System.Linq.Dynamic.Core;

namespace CIS.WebApp.Components.Pages.Admin
{
    public partial class InspectLegacyData : ComponentBase, IDisposable
    {
        [Inject]
        public required SWNDistroContext LegacyDbContext { get; set; }

        private int _legacyOrdersCount;
        private IEnumerable<Ordre> _legacyOrders = Enumerable.Empty<Ordre>();

        private int _legacyStoresCount;
        private IEnumerable<Butikkliste> _legacyStores = Enumerable.Empty<Butikkliste>();

        private int _legacyProductCount;
        private IEnumerable<Vareinfo> _legacyProducts = Enumerable.Empty<Vareinfo>();

        private int _legacySalesStatisticsCount;
        private IEnumerable<Salg> _legacySalesStatistics = Enumerable.Empty<Salg>();

        private CancellationTokenSource _cts = new();

        private async Task LoadLegacySalesStatistics(LoadDataArgs args)
        {
            var query = LegacyDbContext.Salgs
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacySalesStatisticsCount = await query
                .CountAsync(_cts.Token);
            _legacySalesStatistics = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync(_cts.Token);
        }

        private async Task LoadLegacyOrders(LoadDataArgs args)
        {
            var query = LegacyDbContext.Ordres
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacyOrdersCount = await query
                .CountAsync(_cts.Token);
            _legacyOrders = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync(_cts.Token);
        }

        private async Task LoadLegacyStores(LoadDataArgs args)
        {
            var query = LegacyDbContext.Butikklistes
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacyStoresCount = await query
                .CountAsync(_cts.Token);
            _legacyStores = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync(_cts.Token);
        }

        private async Task LoadLegacyProducts(LoadDataArgs args)
        {
            var query = LegacyDbContext.Vareinfos
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _legacyProductCount = await query
                .CountAsync(_cts.Token);
            _legacyProducts = await query
                .Skip(args.Skip ?? 0)
                .Take(args.Top ?? 100)
                .ToListAsync(_cts.Token);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
