namespace GraphShield.Data.Configuration.Models
{
    /// <summary>
    /// Represents the configuration for database migrations.
    /// </summary>
    public class DatabaseMigrationsConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether to apply database migrations.
        /// </summary>
        public bool ApplyDatabaseMigrations { get; set; } = false;

        /// <summary>
        /// Gets the migrations assembly for the DataConfigDbContext.
        /// </summary>
        public string DataConfigDbMigrationsAssembly { get; private set; }

        /// <summary>
        /// Gets the migrations assembly for the DataProtectionDbContext.
        /// </summary>
        public string DataProtectionDbMigrationsAssembly { get; private set; }

        /// <summary>
        /// Sets the migrations assemblies for both DataConfigDbContext and DataProtectionDbContext.
        /// </summary>
        /// <param name="commonMigrationsAssembly">The common migrations assembly to be used for both contexts.</param>
        public void SetMigrationsAssemblies(string commonMigrationsAssembly)
        {
            DataConfigDbMigrationsAssembly = commonMigrationsAssembly;
            DataProtectionDbMigrationsAssembly = commonMigrationsAssembly;
        }
    }
}