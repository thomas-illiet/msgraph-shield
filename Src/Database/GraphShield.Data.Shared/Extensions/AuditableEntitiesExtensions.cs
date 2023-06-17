using GraphShield.Data.Model.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LicenseManager.Api.Data.Shared.Extensions
{
    internal static partial class DbContextExtensions
    {
        /// <summary>
        /// Updates the auditable entities in the context by populating special properties.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="editorUserId">The editor user identifier.</param>
        public static void UpdateAuditableEntities(this DbContext context, Guid editorUserId)
        {
            DateTime utcNow = DateTime.UtcNow;

            var changedEntries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted);

            foreach (var dbEntry in changedEntries)
            {
                UpdateAuditableEntity(dbEntry, utcNow, editorUserId);
            }
        }

        /// <summary>
        /// Updates the special properties for a single auditable entity in the context.
        /// </summary>
        /// <param name="dbEntry">The database entry.</param>
        /// <param name="utcNow">The current UTC time.</param>
        /// <param name="editorUserId">The editor user identifier.</param>
        private static void UpdateAuditableEntity(EntityEntry dbEntry, DateTime utcNow, Guid editorUserId)
        {
            object entity = dbEntry.Entity;

            switch (dbEntry.State)
            {
                case EntityState.Added:

                    if (entity is ICreationAuditable creationAuditable)
                    {
                        UpdateTrackableEntity(dbEntry, utcNow);
                        creationAuditable.CreatedBy = editorUserId;
                    }
                    break;

                case EntityState.Modified:
                    if (entity is IModificationAuditable modificationAuditable)
                    {
                        UpdateTrackableEntity(dbEntry, utcNow);
                        modificationAuditable.UpdatedBy = editorUserId;
                        dbEntry.CurrentValues[nameof(IModificationAuditable.UpdatedBy)] = editorUserId;

                        if (entity is ICreationAuditable)
                        {
                            PreventPropertyOverwrite<string>(dbEntry, nameof(ICreationAuditable.CreatedBy));
                        }
                    }
                    break;

                case EntityState.Deleted:
                    if (entity is IDeletionAuditable deletionAuditable)
                    {
                        UpdateTrackableEntity(dbEntry, utcNow);

                        // change CurrentValues after dbEntry.State becomes EntityState.Unchanged
                        deletionAuditable.DeletedBy = editorUserId;
                        dbEntry.CurrentValues[nameof(IDeletionAuditable.DeletedBy)] = editorUserId;
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}