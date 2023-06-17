using GraphShield.Data.Configuration.Models;
using Microsoft.Extensions.Configuration;

namespace GraphShield.Data
{
    /// <summary>
    /// Represents the configuration settings for the database.
    /// </summary>
    public class DatabaseConfiguration
    {
        /// <summary>
        /// Gets or sets the database connection strings and settings.
        /// </summary>
        public ConnectionStringsConfiguration ConnectionStrings { get; set; } = new ConnectionStringsConfiguration();

        /// <summary>
        /// Gets or sets the settings for the database provider.
        /// </summary>
        public DatabaseProviderConfiguration DatabaseProvider { get; set; } = new DatabaseProviderConfiguration();

        /// <summary>
        /// Gets or sets the settings for database migrations.
        /// </summary>
        public DatabaseMigrationsConfiguration DatabaseMigrations { get; set; } = new DatabaseMigrationsConfiguration();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration instance containing the database settings.</param>
        public DatabaseConfiguration(IConfiguration configuration)
        {
            configuration.GetSection("ConnectionStrings").Bind(ConnectionStrings);
            configuration.GetSection(nameof(DatabaseProviderConfiguration)).Bind(DatabaseProvider);
            configuration.GetSection(nameof(DatabaseMigrationsConfiguration)).Bind(DatabaseMigrations);
        }
    }
}