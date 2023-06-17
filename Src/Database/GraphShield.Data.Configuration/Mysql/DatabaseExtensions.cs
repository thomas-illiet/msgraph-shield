using GraphShield.Data.Configuration.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GraphShield.Data.Configuration.Mysql
{
    /// <summary>
    /// Provides extension methods to register MySQL DbContexts.
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Register MySQL DbContexts.
        /// </summary>
        /// <typeparam name="TDataConfigStoreDbContext">The type of the data config store DbContext.</typeparam>
        /// <typeparam name="TDataProtectionDbContext">The type of the data protection DbContext.</typeparam>
        /// <param name="services">The IServiceCollection.</param>
        /// <param name="connectionStrings">The connection strings configuration.</param>
        /// <param name="databaseMigrations">The database migrations configuration.</param>
        public static void RegisterMysqlDbContexts<TDataConfigStoreDbContext, TDataProtectionDbContext>(
            this IServiceCollection services,
            ConnectionStringsConfiguration connectionStrings,
            DatabaseMigrationsConfiguration databaseMigrations)
            where TDataConfigStoreDbContext : DbContext
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

            // Config DB for DataStore
            services.AddDbContext<TDataConfigStoreDbContext>(options => options.UseMySql(connectionStrings.DataConfigDbContext,
                ServerVersion.AutoDetect(connectionStrings.DataConfigDbContext),
                optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.DataConfigDbMigrationsAssembly ?? migrationsAssembly)));

            // Config DB for DataProtection
            services.AddDbContext<TDataProtectionDbContext>(options => options.UseMySql(connectionStrings.DataProtectionDbContext,
                ServerVersion.AutoDetect(connectionStrings.DataProtectionDbContext),
                optionsSql => optionsSql.MigrationsAssembly(databaseMigrations.DataProtectionDbMigrationsAssembly ?? migrationsAssembly)));
        }
    }
}
