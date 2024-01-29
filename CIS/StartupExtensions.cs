using CIS.DataAccess;

namespace CIS
{
    public static class StartupExtensions
    {
        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.MigrateDataAccess();
            }
        }
    }
}
