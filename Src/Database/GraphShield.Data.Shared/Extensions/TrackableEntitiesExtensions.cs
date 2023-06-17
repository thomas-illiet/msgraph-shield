using GraphShield.Data.Model.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace LicenseManager.Api.Data.Shared.Extensions
{
    internal static partial class DbContextExtensions
    {
        /// <summary>
        /// Updates the trackable entities in the context by populating special properties.
        /// </summary>
        /// <param name="context">The database context.</param>
        public static void UpdateTrackableEntities(this DbContext context)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            var changedEntries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted);

            foreach (var dbEntry in changedEntries)
            {
                UpdateTrackableEntity(dbEntry, utcNow);
            }
        }

        /// <summary>
        /// Updates the special properties for a single entity in the context.
        /// </summary>
        /// <param name="dbEntry">The database entry.</param>
        /// <param name="utcNow">The current UTC time.</param>
        private static void UpdateTrackableEntity(EntityEntry dbEntry, DateTimeOffset utcNow)
        {
            object entity = dbEntry.Entity;

            switch (dbEntry.State)
            {
                case EntityState.Added:
                    if (entity is ICreationTrackable creationTrackable)
                    {
                        creationTrackable.CreatedUtc = utcNow;
                    }
                    break;

                case EntityState.Modified:
                    if (entity is IModificationTrackable modificationTrackable)
                    {
                        modificationTrackable.UpdatedUtc = utcNow;
                        dbEntry.CurrentValues[nameof(IModificationTrackable.UpdatedUtc)] = utcNow;

                        if (entity is ICreationTrackable)
                        {
                            PreventPropertyOverwrite<DateTime>(dbEntry, nameof(ICreationTrackable.CreatedUtc));
                        }
                    }
                    break;

                case EntityState.Deleted:
                    if (entity is ISoftDeletable softDeletable)
                    {
                        dbEntry.State = EntityState.Unchanged;
                        softDeletable.IsDeleted = true;
                        dbEntry.CurrentValues[nameof(ISoftDeletable.IsDeleted)] = true;

                        if (entity is IDeletionTrackable deletionTrackable)
                        {
                            deletionTrackable.DeletedUtc = utcNow;
                            dbEntry.CurrentValues[nameof(IDeletionTrackable.DeletedUtc)] = utcNow;
                        }
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Prevents property overwrite if <see cref="EntityEntry.State"/> is set to <see cref="EntityState.Modified"/>
        /// on an entity with empty <see cref="ICreationTrackable.CreatedUtc"/> or <see cref="ICreationAuditable.CreatedBy"/>.
        /// </summary>
        private static void PreventPropertyOverwrite<TProperty>(EntityEntry dbEntry, string propertyName)
        {
            var propertyEntry = dbEntry.Property(propertyName);

            if (propertyEntry.IsModified && Equals(dbEntry.CurrentValues[propertyName], default(TProperty)))
            {
                propertyEntry.IsModified = false;
            }
        }
    }
}
