using GraphShield.Data.Configuration.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GraphShield.Data
{
    /// <summary>
    /// Helper class for managing database migrations.
    /// </summary>
    public static class DatabaseMigrationHelpers
    {
        /// <summary>
        /// Applies the database migrations asynchronously.
        /// </summary>
        /// <typeparam name="TDataConfigDbContext">The type of the data config DbContext.</typeparam>
        /// <typeparam name="TDataProtectionDbContext">The type of the data protection DbContext.</typeparam>
        /// <param name="host">The IHost instance representing the host application.</param>
        /// <param name="databaseMigrationsConfiguration">The database migrations configuration.</param>
        /// <returns>A boolean value indicating whether the migrations were applied successfully.</returns>
        public static async Task<bool> ApplyDbMigrationsAsync<TDataConfigDbContext, TDataProtectionDbContext>(
            IHost host,
            DatabaseMigrationsConfiguration databaseMigrationsConfiguration)
            where TDataConfigDbContext : DbContext
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                if (databaseMigrationsConfiguration?.ApplyDatabaseMigrations == true)
                {
                    return await EnsureDatabasesMigratedAsync<TDataConfigDbContext, TDataProtectionDbContext>(services);
                }
            }

            return false;
        }

        /// <summary>
        /// Ensures that the databases are migrated asynchronously.
        /// </summary>
        /// <typeparam name="TDataConfigDbContext">The type of the data config DbContext.</typeparam>
        /// <typeparam name="TDataProtectionDbContext">The type of the data protection DbContext.</typeparam>
        /// <param name="services">The IServiceProvider instance representing the service provider.</param>
        /// <returns>A boolean value indicating whether the migrations were applied successfully.</returns>
        public static async Task<bool> EnsureDatabasesMigratedAsync<TDataConfigDbContext, TDataProtectionDbContext>(IServiceProvider services)
            where TDataConfigDbContext : DbContext
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var migrationComplete = true;
                var pendingMigrationCount = 0;
                var dbContextTypes = new Type[] { typeof(TDataConfigDbContext), typeof(TDataProtectionDbContext) };

                foreach (var dbContextType in dbContextTypes)
                {
                    var context = scope.ServiceProvider.GetRequiredService(dbContextType) as DbContext;
                    await context!.Database.MigrateAsync();
                    pendingMigrationCount += (await context.Database.GetPendingMigrationsAsync()).Count();
                }

                migrationComplete = pendingMigrationCount == 0;
                return migrationComplete;
            }
        }
    }
}
