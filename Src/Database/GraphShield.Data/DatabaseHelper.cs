using GraphShield.Data.Configuration.Mysql;
using GraphShield.Data.Configuration.PostgreSQL;
using GraphShield.Data.Configuration.SqlServer;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MysqlMigrationAssembly = GraphShield.Data.Mysql.Helpers.MigrationAssembly;
using PostgreSQLMigrationAssembly = GraphShield.Data.PostgreSQL.Helpers.MigrationAssembly;
using SqlMigrationAssembly = GraphShield.Data.SQLServer.Helpers.MigrationAssembly;

namespace GraphShield.Data
{
    /// <summary>
    /// Helper class for database operations and configurations.
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Registers the DbContexts for this client based on the provided configuration.
        /// Configure the connection strings in AppSettings.json.
        /// </summary>
        /// <typeparam name="TDataConfigDbContext">The type of the data config DbContext.</typeparam>
        /// <typeparam name="TDataProtectionDbContext">The type of the data protection DbContext.</typeparam>
        /// <param name="services">The IServiceCollection to register the DbContexts with.</param>
        /// <param name="configuration">The IConfiguration instance containing the database configuration settings.</param>
        public static void RegisterDbContexts<TDataConfigDbContext, TDataProtectionDbContext>(this IServiceCollection services, IConfiguration configuration)
            where TDataConfigDbContext : DbContext
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            var config = new DatabaseConfiguration(configuration);
            services.RegisterDbContexts<TDataConfigDbContext, TDataProtectionDbContext>(config);
        }

        /// <summary>
        /// Registers the database contexts for production based on the provided configuration.
        /// </summary>
        /// <typeparam name="TDataConfigDbContext">The type of the data config DbContext.</typeparam>
        /// <typeparam name="TDataProtectionDbContext">The type of the data protection DbContext.</typeparam>
        /// <param name="services">The IServiceCollection to register the DbContexts with.</param>
        /// <param name="configuration">The DatabaseConfiguration instance containing the database configuration settings.</param>
        public static void RegisterDbContexts<TDataConfigDbContext, TDataProtectionDbContext>(this IServiceCollection services, DatabaseConfiguration configuration)
            where TDataConfigDbContext : DbContext
            where TDataProtectionDbContext : DbContext, IDataProtectionKeyContext
        {
            // Set Migration assembly
            var migrationsAssembly = GetMigrationAssemblyByProvider(configuration.DatabaseProvider);
            configuration.DatabaseMigrations.SetMigrationsAssemblies(migrationsAssembly);

            // Register the database provider.
            switch (configuration.DatabaseProvider.ProviderType)
            {
                case DatabaseProviderType.SqlServer:
                    services.RegisterSqlServerDbContexts<TDataConfigDbContext, TDataProtectionDbContext>(configuration.ConnectionStrings, configuration.DatabaseMigrations);
                    break;

                case DatabaseProviderType.PostgreSQL:
                    services.RegisterNpgSqlDbContexts<TDataConfigDbContext, TDataProtectionDbContext>(configuration.ConnectionStrings, configuration.DatabaseMigrations);
                    break;

                case DatabaseProviderType.MySql:
                    services.RegisterMysqlDbContexts<TDataConfigDbContext, TDataProtectionDbContext>(configuration.ConnectionStrings, configuration.DatabaseMigrations);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(configuration.DatabaseProvider.ProviderType), $@"The value needs to be one of {string.Join(", ", Enum.GetNames(typeof(DatabaseProviderType)))}.");
            }
        }

        /// <summary>
        /// Gets the migration assembly name based on the specified database provider.
        /// </summary>
        /// <param name="databaseProvider">The DatabaseProviderConfiguration instance representing the database provider.</param>
        /// <returns>The name of the migration assembly.</returns>
        private static string GetMigrationAssemblyByProvider(DatabaseProviderConfiguration databaseProvider)
        {
            if (databaseProvider.ProviderType == DatabaseProviderType.SqlServer)
                return typeof(SqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name!;
            if (databaseProvider.ProviderType == DatabaseProviderType.PostgreSQL)
                return typeof(PostgreSQLMigrationAssembly).GetTypeInfo().Assembly.GetName().Name!;
            if (databaseProvider.ProviderType == DatabaseProviderType.MySql)
                return typeof(MysqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name!;

            throw new ArgumentOutOfRangeException(
                nameof(databaseProvider.ProviderType),
                $"The value needs to be one of {string.Join(", ", Enum.GetNames(typeof(DatabaseProviderType)))}.");
        }
    }
}
