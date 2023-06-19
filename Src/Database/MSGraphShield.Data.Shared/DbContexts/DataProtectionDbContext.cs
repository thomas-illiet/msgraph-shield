using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MSGraphShield.Data.Shared.DbContexts
{
    /// <summary>
    /// Represents a session with the data protection database.
    /// </summary>
    /// <seealso cref="DbContext" />
    /// <seealso cref="IDataProtectionKeyContext" />
    public class DataProtectionDbContext : DbContext, IDataProtectionKeyContext
    {
        /// <summary>
        /// Gets or sets the data protection keys.
        /// </summary>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProtectionDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public DataProtectionDbContext(DbContextOptions<DataProtectionDbContext> options)
            : base(options) { }
    }
}