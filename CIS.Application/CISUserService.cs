using CIS.Application.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application
{
    public interface ICISUserService
    {
        Task<ApplicationUserView> GetCurrentUser(CancellationToken cancellationToken);
        Task<Guid> GetCurrentStoreId(CancellationToken cancellationToken);
    }

    internal sealed class CISUserService : ICISUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CISDbContext _dbContext;

        public CISUserService(
            UserManager<ApplicationUser> userManager,
            CISDbContext dbContext,
            IHttpContextAccessor httpContextAccessor = null)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public async Task<ApplicationUserView> GetCurrentUser(
            CancellationToken cancellationToken)
        {
            if (_httpContextAccessor?.HttpContext?.User is null)
                throw new Exception("Must be signed on");

            var currentUser = await _userManager
                .GetUserAsync(_httpContextAccessor.HttpContext.User);
            if(currentUser is null)
                throw new Exception("Must be signed on");

            var customer = currentUser.CustomerId.HasValue ?
                await _dbContext.Customers
                    .AsNoTracking()    
                    .FirstOrDefaultAsync(x => x.Id == currentUser.CustomerId, cancellationToken) : null;

            var userView = new ApplicationUserView()
            {
                Id = currentUser.Id,
                UserName = currentUser.UserName,
                NormalizedUserName = currentUser.NormalizedUserName,
                Email = currentUser.Email,
                NormalizedEmail = currentUser.NormalizedEmail,
                EmailConfirmed = currentUser.EmailConfirmed,
                PasswordHash = currentUser.PasswordHash,
                AccessFailedCount = currentUser.AccessFailedCount,
                ConcurrencyStamp = currentUser.ConcurrencyStamp,    
                LockoutEnabled = currentUser.LockoutEnabled,
                LockoutEnd = currentUser.LockoutEnd,
                PhoneNumber = currentUser.PhoneNumber,
                PhoneNumberConfirmed = currentUser.PhoneNumberConfirmed,
                SecurityStamp = currentUser.SecurityStamp,
                TwoFactorEnabled = currentUser.TwoFactorEnabled,
                IsAdmin = currentUser.IsAdmin,
                CustomerId = currentUser.CustomerId,
                CustomerNumber = customer?.Number,
                CustomerName = customer?.Name
            };


            return userView;
        }

        public async Task<Guid> GetCurrentStoreId(CancellationToken cancellationToken)
        {
            var currentUser = await GetCurrentUser(cancellationToken);
            var storeId = await _dbContext.Stores
                .Where(x => x.OwnerCustomerId == currentUser.CustomerId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return storeId;
        }
    }
}
