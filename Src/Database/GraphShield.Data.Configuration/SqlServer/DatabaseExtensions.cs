using GraphShield.Data.Configuration.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GraphShield.Data.Configuration.SqlServer
{
    /// <summary>
    /// Provides extension methods to register SQL Server DbContexts.
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Register SQL Server DbContexts.
        /// </summary>
        /// <typeparam name="TDataConfigDbContext">The type of the data config DbContext.</typeparam>
        /// <typeparam name="TDataProtectionDbContext">The type of the data protection DbContext.</typeparam>
        /// <param name="services">The IServiceCollection.</param>
        /// <param name="connectionStrings">The connection strings configuration.</param>
        /// <param name="databaseMigrations">The database migrations configuration.</param>
        public static void RegisterSqlServerDbContexts<TDataConfigDbContext, TDataProtectionDbContext>(
            this IServiceCollection services,
            ConnectionStringsConfiguration connectionStrings,
            DatabaseMigrationsConfiguration databaseMigrations)
            where TDataConfigDbContext : DbContext
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

            // Config DB for DataStore
            services.AddDbContext<TDataConfigDbContext>(options =>
                options.UseSqlServer(connectionStrings.DataConfigDbContext,
                optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.DataConfigDbMigrationsAssembly ?? migrationsAssembly)));

            // Config DB for DataProtection
            services.AddDbContext<TDataProtectionDbContext>(options =>
                options.UseSqlServer(connectionStrings.DataProtectionDbContext,
                optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.DataProtectionDbMigrationsAssembly ?? migrationsAssembly)));
        }
    }
}
