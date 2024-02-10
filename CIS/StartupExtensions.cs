using CIS.Application;

namespace CIS
{
    public static class StartupExtensions
    {
        public static void InitializeDatabase(
            this IApplicationBuilder app, bool requiresMigrationFromLegacy)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.MigrateDataAccess(requiresMigrationFromLegacy);
            }
        }
    }
}
