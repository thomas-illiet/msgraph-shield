using MSGraphShield.Data;
using MSGraphShield.Data.Shared.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MSGraphShield.Ext.HealthCheck
{
    public static class DatabaseHealthCheck
    {
        /// <summary>
        /// Adds the database health checks.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static IHealthChecksBuilder AddDatabaseChecks(
            this IHealthChecksBuilder builder, IServiceCollection services, IConfiguration configuration)
        {
            // Retrieve all connection strings
            var dataConfigDbConnectionString = configuration.GetConnectionString("DataConfigDbContext");
            var dataProtectionDbConnectionString = configuration.GetConnectionString("DataProtectionDbContext");

            if (string.IsNullOrWhiteSpace(dataConfigDbConnectionString) || string.IsNullOrWhiteSpace(dataProtectionDbConnectionString))
                throw new ArgumentException("One or more connection strings are null or empty.");

            // Setup the database context health check
            builder.AddDbContextCheck<DataConfigDbContext>("DataConfigDbContext");
            builder.AddDbContextCheck<DataProtectionDbContext>("DataProtectionDbContext");

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                // Retrieve first table name of an entity in the given DbContext
                var dataConfigTableName = GetEntityTable<DataConfigDbContext>(scope.ServiceProvider);
                var dataProtectionTableName = GetEntityTable<DataProtectionDbContext>(scope.ServiceProvider);

                // Setup the database health check
                var databaseProvider = configuration.GetSection(nameof(DatabaseProviderConfiguration)).Get<DatabaseProviderConfiguration>();
                switch (databaseProvider.ProviderType)
                {
                    case DatabaseProviderType.SqlServer:
                        builder.AddSqlServer(dataConfigDbConnectionString, name: "DataConfigDb",
                            healthQuery: $"SELECT TOP 1 * FROM dbo.[{dataConfigTableName}]");
                        builder.AddSqlServer(dataProtectionDbConnectionString, name: "DataProtectionDb",
                            healthQuery: $"SELECT TOP 1 * FROM dbo.[{dataProtectionTableName}]");
                        break;

                    case DatabaseProviderType.PostgreSQL:
                        builder.AddNpgSql(dataConfigDbConnectionString, name: "DataConfigDb",
                            healthQuery: $"SELECT * FROM \"{dataConfigTableName}\" LIMIT 1");
                        builder.AddNpgSql(dataProtectionDbConnectionString, name: "DataProtectionDb",
                            healthQuery: $"SELECT * FROM \"{dataProtectionTableName}\" LIMIT 1");
                        break;

                    case DatabaseProviderType.MySql:
                        builder.AddMySql(dataConfigDbConnectionString, name: "DataConfigDb");
                        builder.AddMySql(dataProtectionDbConnectionString, name: "DataProtectionDb");
                        break;

                    default:
                        throw new NotImplementedException($"Health checks not defined for database provider {databaseProvider.ProviderType}");
                }
            }

            return builder;
        }

        /// <summary>
        /// Get the table name of an entity in the given DbContext.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="entityTypeName">The full name of the entity type (optional).</param>
        private static string? GetEntityTable<TDbContext>(IServiceProvider serviceProvider, string entityTypeName = null)
            where TDbContext : DbContext
        {
            var db = serviceProvider.GetService<TDbContext>();
            if (db != null)
            {
                var entityType = entityTypeName != null ? db.Model.FindEntityType(entityTypeName) : db.Model.GetEntityTypes().FirstOrDefault();
                if (entityType != null)
                    return entityType.GetTableName();
            }

            return null;
        }
    }
}
