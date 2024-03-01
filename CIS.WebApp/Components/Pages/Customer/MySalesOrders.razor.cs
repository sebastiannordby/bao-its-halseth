using CIS.Application;
using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Infrastructure;
using CIS.Application.Shared;
using CIS.WebApp.Components.Pages.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Radzen;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace CIS.WebApp.Components.Pages.Customer
{
    public partial class MySalesOrders : ComponentBase, IDisposable
    {
        [Inject]
        public required ISalesQueries SalesQueries { get; set; }

        [Inject]
        public required ICISUserService CISUserService { get; set; }

        private int _salesOrderCount;
        private List<SalesOrderView>? _salesOrders;
        private CancellationTokenSource _cts = new();
        private ApplicationUserView? _currentUser;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await CISUserService
                .GetCurrentUser(_cts.Token);
        }

        private async Task LoadSalesOrders(LoadDataArgs args)
        {
            if (_currentUser is null || !_currentUser.CustomerNumber.HasValue)
                return;

            var query = SalesQueries.Query()
                .Where(x => x.CustomerNumber == _currentUser.CustomerNumber.Value);

            if (!string.IsNullOrEmpty(args.Filter))
            {
                query = query.Where(args.Filter);
            }

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                query = query.OrderBy(args.OrderBy);
            }

            _salesOrderCount = await query
                .CountAsync(_cts.Token);
            _salesOrders = await query
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
