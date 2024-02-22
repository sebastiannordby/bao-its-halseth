using CIS.Application;

namespace CIS.WebApp.Extensions
{
    public static class StartupExtensions
    {
        public static async Task InitializeDatabase(
            this IApplicationBuilder app, 
            bool requiresMigrationFromLegacy, 
            bool insertTestUser)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                await serviceScope.ServiceProvider.ApplyCISDataMigrations(
                    requiresMigrationFromLegacy, insertTestUser);
            }
        }
    }
}
