namespace GraphShield.Data.Configuration.Models
{
    /// <summary>
    /// Represents the configuration for database connection strings.
    /// </summary>
    public class ConnectionStringsConfiguration
    {
        /// <summary>
        /// Gets or sets the connection string for the DataConfigDbContext.
        /// </summary>
        public string DataConfigDbContext { get; set; }

        /// <summary>
        /// Gets or sets the connection string for the DataProtectionDbContext.
        /// </summary>
        public string DataProtectionDbContext { get; set; }
    }
}