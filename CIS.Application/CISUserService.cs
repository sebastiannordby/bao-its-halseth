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
    public sealed class CISUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CISDbContext _dbContext;

        public CISUserService(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            CISDbContext dbContext)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public async Task<ApplicationUser> GetCurrentUser()
        {
            if (_httpContextAccessor?.HttpContext?.User is null)
                throw new Exception("Must be signed on");

            var currentUser = await _userManager
                .GetUserAsync(_httpContextAccessor.HttpContext.User);
            if(currentUser is null)
                throw new Exception("Must be signed on");

            return currentUser;
        }

        public async Task<Guid> GetCurrentStoreId()
        {
            var currentUser = await GetCurrentUser();
            var storeId = await _dbContext.Stores
                .Where(x => x.OwnerCustomerId == currentUser.CustomerId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            return storeId;
        }
    }
}
