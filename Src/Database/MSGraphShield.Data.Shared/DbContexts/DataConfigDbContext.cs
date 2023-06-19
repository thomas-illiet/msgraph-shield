using MSGraphShield.Data.Configuration.Builders;
using MSGraphShield.Data.Model.Entities;
using LicenseManager.Api.Data.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MSGraphShield.Data.Shared.DbContexts
{
    /// <summary>
    /// Represents a session with the data store database.
    /// </summary>
    /// <seealso cref="DbContext" />
    public class DataConfigDbContext : DbContext
    {
        public DbSet<ClientEntity> Applications { get; set; } = null!;
        public DbSet<CredentialEntity> Credentials { get; set; } = null!;
        public DbSet<RuleEntity> Rules { get; set; } = null!;
        public DbSet<ProfileEntity> Profiles { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConfigDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public DataConfigDbContext(DbContextOptions<DataConfigDbContext> options) : base(options)
        { }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ClientProfileBuilder());
            modelBuilder.ApplyConfiguration(new ClientBuilder());
            modelBuilder.ApplyConfiguration(new CredentialBuilder());
            modelBuilder.ApplyConfiguration(new RuleBuilder());
            modelBuilder.ApplyConfiguration(new ProfileBuilder());
            modelBuilder.ApplyConfiguration(new UserBuilder());
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<int> SaveChangesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            this.UpdateAuditableEntities(userId);
            return SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) 
            => SaveChangesAsync(true, cancellationToken);

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.UpdateTrackableEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
