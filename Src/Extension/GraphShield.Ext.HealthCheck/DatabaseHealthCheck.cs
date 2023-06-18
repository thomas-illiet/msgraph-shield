using GraphShield.Data;
using GraphShield.Data.Shared.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GraphShield.Ext.HealthCheck
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
            this IHealthChecksBuilder builder,
            IServiceCollection services, IConfiguration configuration)
        {
            // Retrieve all connection strings
            var persistedDataDbConnectionString = configuration.GetConnectionString("PersistedDataDbContext");
            var dataProtectionDbConnectionString = configuration.GetConnectionString("DataProtectionDbContext");

            // Setup the database context health check
            var healthChecksBuilder = builder.AddDbContextCheck<DataConfigDbContext>("PersistedDataDbContext")
                .AddDbContextCheck<DataProtectionDbContext>("DataProtectionDbContext");

            var serviceProvider = services.BuildServiceProvider();
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                // Retrieve first table name of an entity in the given DbContext
                var persistedDataTableName = GetEntityTable<DataConfigDbContext>(scope.ServiceProvider);
                var dataProtectionTableName = GetEntityTable<DataProtectionDbContext>(scope.ServiceProvider);

                // Setup the database health check
                var databaseProvider = configuration.GetSection(nameof(DatabaseProviderConfiguration)).Get<DatabaseProviderConfiguration>();
                switch (databaseProvider.ProviderType)
                {
                    case DatabaseProviderType.SqlServer:
                        healthChecksBuilder
                            .AddSqlServer(persistedDataDbConnectionString, name: "PersistedDataDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{persistedDataTableName}]")
                            .AddSqlServer(dataProtectionDbConnectionString, name: "DataProtectionDb",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{dataProtectionTableName}]");

                        break;

                    case DatabaseProviderType.PostgreSQL:
                        healthChecksBuilder
                            .AddNpgSql(persistedDataDbConnectionString, name: "PersistedDataDb",
                                healthQuery: $"SELECT * FROM \"{persistedDataTableName}\" LIMIT 1")
                            .AddNpgSql(dataProtectionDbConnectionString, name: "DataProtectionDb",
                                healthQuery: $"SELECT * FROM \"{dataProtectionTableName}\" LIMIT 1");
                        break;

                    case DatabaseProviderType.MySql:
                        healthChecksBuilder
                            .AddMySql(persistedDataDbConnectionString, name: "PersistedDataDb")
                            .AddMySql(dataProtectionDbConnectionString, name: "DataProtectionDb");

                        break;

                    default:
                        throw new NotImplementedException($"Health checks not defined for database provider {databaseProvider.ProviderType}");
                }

                return builder;
            }
        }

        /// <summary>
        /// Get the table name of an entity in the given DbContext
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="entityTypeName">If specified, the full name of the type of the entity.
        /// Otherwise, the first entity in the DbContext will be retrieved</param>
        private static string? GetEntityTable<TDbContext>(IServiceProvider serviceProvider, string entityTypeName = null)
            where TDbContext : DbContext
        {
            var db = serviceProvider.GetService<TDbContext>();
            if (db != null)
            {
                var entityType = entityTypeName != null ? db.Model.FindEntityType(entityTypeName) : db.Model.GetEntityTypes().FirstOrDefault();
                if (entityType != null)
                    return entityType.GetTableName()!;
            }

            return default;
        }
    }
}